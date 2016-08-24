
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

namespace RosebudAppAndroid.Activities
{
    [Activity(Label = "FeedDetailsActivity", ParentActivity = typeof(MainActivity))]
    public class FeedDetailsActivity : AppCompatActivity
    {
        private const string STATE_RECYCLER_VIEW = "state-recycler-view";

        Feed feedInfo;

        AppBarLayout appBarLayout;
        TextView lblToolbarDate;
        RouteAdapter routeAdapter;
        RecyclerView routeRecyclerView;
        SwipeRefreshLayout routePullToRefresh;
        SwipeRefreshLayout routePullToRefreshEmpty;
        IMenuItem searchMenu;

        ImageView icoDropdownDatePicker;
        bool isCalendarExpanded;
        float currentCalendarArrowRotation = 360f;

        IParcelable recyclerViewLayoutState;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.feed_details);

            appBarLayout = FindViewById<AppBarLayout>(Resource.Id.app_bar_layout);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.my_awesome_toolbar);
            var lblToolbarTitle = FindViewById<TextView>(Resource.Id.lbl_toolbar_title);
            lblToolbarDate = FindViewById<TextView>(Resource.Id.lbl_toolbar_date);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            NetworkStatusFragment networkFragment = (NetworkStatusFragment)FragmentManager.FindFragmentById(Resource.Id.network_fragment);

            routeRecyclerView = FindViewById<RecyclerView>(Resource.Id.route_recyclerview);
            routeRecyclerView.NestedScrollingEnabled = false;
            routePullToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.route_pull_to_refresh);
            routePullToRefreshEmpty = FindViewById<SwipeRefreshLayout>(Resource.Id.route_pull_to_refresh_empty);

            var btnDatePicker = FindViewById<RelativeLayout>(Resource.Id.btn_date_picker);
            icoDropdownDatePicker = FindViewById<ImageView>(Resource.Id.ico_dropdown_calendar);
            var calendarView = FindViewById<CalendarView>(Resource.Id.calendar_view);

            feedInfo = JsonConvert.DeserializeObject<Feed>(Intent.GetStringExtra("feedInfos"));
            lblToolbarTitle.Text = feedInfo.agency_name;

            routePullToRefresh.SetColorSchemeResources(Resource.Color.refresh_progress_1, Resource.Color.refresh_progress_2, Resource.Color.refresh_progress_3);
            routePullToRefreshEmpty.SetColorSchemeResources(Resource.Color.refresh_progress_1, Resource.Color.refresh_progress_2, Resource.Color.refresh_progress_3);

            toolbar.NavigationClick += delegate
            {
                OnBackPressed();
            };

            routePullToRefresh.Refresh += async delegate
            {
                await LoadRoutes(true);
            };

            routePullToRefreshEmpty.Refresh += async delegate
            {
                await LoadRoutes(true);
            };

            btnDatePicker.Click += delegate
            {
                ToggleDatePicker();
            };

            calendarView.Post(async () =>
            {
                long epochTime = (long)(Dependency.PreferenceManager.SelectedDatetime.AddDays(1) - new DateTime(1970, 1, 1)).TotalMilliseconds;
                calendarView.SetDate(epochTime, false, true);
                await SwitchDate(Dependency.PreferenceManager.SelectedDatetime);
            });

            calendarView.DateChange += async (object sender, CalendarView.DateChangeEventArgs e) =>
            {
                ToggleDatePicker();
                await SwitchDate(e.Year, e.Month + 1, e.DayOfMonth);
            };
        
            networkFragment.RetryLastRequest += async (object sender, EventArgs args) =>
            {
                await LoadRoutes();
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);

            searchMenu = menu.FindItem(Resource.Id.action_search);
            searchMenu.SetVisible(routeAdapter != null);

            var searchViewJava = MenuItemCompat.GetActionView(searchMenu);
            SearchViewCompat searchView = searchViewJava.JavaCast<SearchViewCompat>();
            //searchView.QueryTextSubmit

            searchView.QueryTextChange += (sender, args) =>
            {
                routeAdapter.Filter = args.NewText;
            };

            return true;
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutParcelable(STATE_RECYCLER_VIEW, routeRecyclerView.GetLayoutManager().OnSaveInstanceState());
            base.OnSaveInstanceState(outState);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            recyclerViewLayoutState = (IParcelable)savedInstanceState.GetParcelable(STATE_RECYCLER_VIEW);
            base.OnRestoreInstanceState(savedInstanceState);
        }

        private async Task SwitchDate(int year, int month, int day)
        {
            await SwitchDate(new DateTime(year, month, day));
        }

        private async Task SwitchDate(DateTime date)
        {
            Dependency.PreferenceManager.SelectedDatetime = date;
            lblToolbarDate.Text = TimeFormatter.ToFullShortDate(date);
            if (routeAdapter != null)
                routeAdapter.ClearItems();
            await LoadRoutes();
        }

        private async Task LoadRoutes(bool overrideCache = false)
        {
            routePullToRefresh.Refreshing = true;
            routePullToRefreshEmpty.Refreshing = true;

            var routes = await RouteAccessor.GetAllRoutes(feedInfo.feed_id, overrideCache);

            routePullToRefresh.Visibility = routes == null ? ViewStates.Gone : ViewStates.Visible;
            routePullToRefreshEmpty.Visibility = routes == null ? ViewStates.Visible : ViewStates.Gone;
            

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
                if (routes == null)
                {
                    routeAdapter.ClearItems();
                }
                else
                {
                    routeAdapter.ReplaceItems(routes);
                }
            }

            routePullToRefresh.Refreshing = false;
            routePullToRefreshEmpty.Refreshing = false;
        }

        public void OnItemClick(object sender, int position)
        {
            Route clickedRoute = routeAdapter[position];
            Intent detailsIntent = new Intent(this, typeof(RouteDetailsActivity));
            detailsIntent.PutExtra("routeInfos", JsonConvert.SerializeObject(clickedRoute));

            StartActivity(detailsIntent);
        }

        private void ToggleDatePicker()
        {
            RotateAnimation animation = ArrowRotateAnimation.GetAnimation(currentCalendarArrowRotation, 180);
            icoDropdownDatePicker.StartAnimation(animation);

            currentCalendarArrowRotation = (currentCalendarArrowRotation + 180f) % 360f;

            appBarLayout.SetExpanded(!isCalendarExpanded, true);
            isCalendarExpanded = !isCalendarExpanded;
        }
    }
}
