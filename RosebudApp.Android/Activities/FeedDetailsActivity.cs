
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using RosebudAppAndroid.Adapters;
using RosebudAppCore;
using RosebudAppCore.DataAccessor;
using RosebudAppCore.Model;
using Newtonsoft.Json;
using SearchViewCompat = Android.Support.V7.Widget.SearchView;
using RosebudAppAndroid.Fragments;
using RosebudAppAndroid.Utils;
using RosebudAppCore.Utils;
using Android;
using Android.Content.PM;
using RosebudApp.AndroidMaterialCalendarBinding;
using RosebudAppAndroid.Views;

namespace RosebudAppAndroid.Activities
{
    [Activity(Label = "FeedDetailsActivity", ParentActivity = typeof(MainActivity))]
    public class FeedDetailsActivity : CalendarToolBarActivity
    {
        const string STATE_RECYCLER_VIEW = "state-recycler-view";

        Feed feedInfo;

        RouteAdapter routeAdapter;
        RecyclerView routeRecyclerView;
        View emptyView;
        LoadingContainer loadingContainer;
        IMenuItem searchMenu;

        IParcelable recyclerViewLayoutState;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            ActivityLayout = Resource.Layout.feed_details;
            base.OnCreate(savedInstanceState);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            
            NetworkStatusFragment networkFragment = (NetworkStatusFragment)SupportFragmentManager.FindFragmentById(Resource.Id.network_fragment);

            routeRecyclerView = FindViewById<RecyclerView>(Resource.Id.route_recyclerview);
            //This line enable smooth scrolling inside the LoadingContainer
            ViewCompat.SetNestedScrollingEnabled(routeRecyclerView, false);
            emptyView = FindViewById<View>(Resource.Id.empty_view);
            loadingContainer = FindViewById<LoadingContainer>(Resource.Id.loading_container);

            feedInfo = JsonConvert.DeserializeObject<Feed>(Intent.GetStringExtra("feedInfos"));
            lblToolbarTitle.Text = feedInfo.agency_name;

            loadingContainer.Refresh += PullToRefreshRefresh;

            SwitchDate(Dependency.PreferenceManager.SelectedDatetime);

            networkFragment.RetryLastRequest += async (object sender, EventArgs args) =>
            {
                loadingContainer.Loading = true;
                await LoadRoutes();
                loadingContainer.Loading = false;
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);

            searchMenu = menu.FindItem(Resource.Id.action_search);
            searchMenu.SetVisible(routeAdapter != null);

            var searchViewJava = MenuItemCompat.GetActionView(searchMenu);
            SearchViewCompat searchView = searchViewJava.JavaCast<SearchViewCompat>();

            searchView.QueryTextChange += (sender, args) =>
            {
                routeAdapter.Filter = args.NewText;
            };

            return true;
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            if (routeRecyclerView != null)
            {
                outState.PutParcelable(STATE_RECYCLER_VIEW, routeRecyclerView.GetLayoutManager().OnSaveInstanceState());
            }
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            recyclerViewLayoutState = (IParcelable)savedInstanceState.GetParcelable(STATE_RECYCLER_VIEW);
            base.OnRestoreInstanceState(savedInstanceState);
        }

        async Task LoadRoutes(bool overrideCache = false)
        {
            var routes = await RouteAccessor.GetAllRoutes(feedInfo.feed_id, overrideCache);

            emptyView.Visibility = routes == null ? ViewStates.Visible : ViewStates.Gone;

            if (routeAdapter == null)
            {
                if (routes != null)
                {
                    routeAdapter = new RouteAdapter(this, routes);
                    routeAdapter.ItemClick += OnItemClick;
                    routeRecyclerView.SetAdapter(routeAdapter);
                    routeRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
                    InvalidateOptionsMenu();

                    if (recyclerViewLayoutState != null)
                    {
                        routeRecyclerView.GetLayoutManager().OnRestoreInstanceState(recyclerViewLayoutState);
                    }
                }
            }
            else
            {
                routeAdapter.ReplaceItems(routes);
            }
        }

        public void OnItemClick(object sender, int position)
        {
            Route clickedRoute = routeAdapter[position];

            Intent stopSelectionIntent = new Intent(this, typeof(StopSelectionActivity));
            stopSelectionIntent.PutExtra("routeInfos", JsonConvert.SerializeObject(clickedRoute));

            StartActivity(stopSelectionIntent);
        }

        private async void PullToRefreshRefresh(object sender, EventArgs e)
        {
            loadingContainer.Refreshing = true;
            await LoadRoutes(true);
            loadingContainer.Refreshing = false;
        }

        protected override async void OnSelectedDateChanged(object sender, DateTime selectedDate)
        {
            loadingContainer.Loading = true;
            if (routeAdapter != null)
                routeAdapter.ClearItems();
            await LoadRoutes();
            loadingContainer.Loading = false;
        }
    }
}
