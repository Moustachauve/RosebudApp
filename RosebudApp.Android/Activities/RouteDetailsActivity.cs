
using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using RosebudAppAndroid.Fragments;
using RosebudAppCore;
using RosebudAppCore.DataAccessor;
using RosebudAppCore.Model;
using Newtonsoft.Json;
using ToolbarCompat = Android.Support.V7.Widget.Toolbar;
using RosebudAppAndroid.Utils;
using RosebudAppCore.Utils;
using RosebudAppCore.Model.Enum;
using RosebudApp.AndroidMaterialCalendarBinding;
using Java.Util;

namespace RosebudAppAndroid.Activities
{
    [Activity(Label = "RouteDetailsActivity"/*, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize*/)]
    public class RouteDetailsActivity : CalendarToolBarActivity
    {
        Route routeInfo;
        TripDirectionPagerAdapter tripDirectionPagerAdapter;
        TabLayout tabLayout;

        ViewPager viewPager;
        LinearLayout emptyView;
        LinearLayout emptyViewNoInternet;
        TextView emptyViewMainText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            ActivityLayout = Resource.Layout.route_details;
            base.OnCreate(savedInstanceState);

            NetworkStatusFragment networkFragment = (NetworkStatusFragment)FragmentManager.FindFragmentById(Resource.Id.network_fragment);

            tabLayout = FindViewById<TabLayout>(Resource.Id.tab_layout);
            viewPager = FindViewById<ViewPager>(Resource.Id.view_pager);
            emptyView = FindViewById<LinearLayout>(Resource.Id.empty_view);
            emptyViewNoInternet = FindViewById<LinearLayout>(Resource.Id.empty_view_no_internet);
            emptyViewMainText = FindViewById<TextView>(Resource.Id.empty_view_main_text);

            routeInfo = JsonConvert.DeserializeObject<Route>(Intent.GetStringExtra("routeInfos"));
            lblToolbarTitle.Text = routeInfo.route_short_name + " " + routeInfo.route_long_name;

            networkFragment.RetryLastRequest += async (object sender, EventArgs args) =>
            {
                await LoadDetails();
            };

            SwitchDate(Dependency.PreferenceManager.SelectedDatetime);
        }

        protected override void OnDestroy()
        {
            if (tripDirectionPagerAdapter != null)
            {
                tripDirectionPagerAdapter.ItemClicked -= OnItemClicked;
            }

            base.OnDestroy();
        }

        async Task LoadDetails(bool overrideCache = false)
        {
            RouteDetails currentRouteDetails = await RouteAccessor.GetRouteDetails(routeInfo.feed_id, routeInfo.route_id, Dependency.PreferenceManager.SelectedDatetime, overrideCache);

            UpdateEmptyMessage(currentRouteDetails);
            SetDirectionTabs(currentRouteDetails);

            if (tripDirectionPagerAdapter == null)
            {
                tripDirectionPagerAdapter = new TripDirectionPagerAdapter(SupportFragmentManager);
                tripDirectionPagerAdapter.ItemClicked += OnItemClicked;
                viewPager.Adapter = tripDirectionPagerAdapter;
                tabLayout.SetupWithViewPager(viewPager);
            }

            tripDirectionPagerAdapter.UpdateTrips(currentRouteDetails);

            InvalidateOptionsMenu();
        }

        void SetDirectionTabs(RouteDetails routeDetails)
        {
            tabLayout.RemoveAllTabs();

            if (routeDetails != null)
            {
                if (TripDirectionHelper.HasMultipleDirection(routeDetails.trips))
                {
                    tabLayout.Visibility = ViewStates.Visible;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(TripDirectionHelper.GetDirectionName(routeDetails.trips, TripDirection.AnyDirection)))
                        tabLayout.Visibility = ViewStates.Gone;

                    else
                    {
                        tabLayout.Visibility = ViewStates.Visible;
                    }
                }
            }
            else
            {
                tabLayout.Visibility = ViewStates.Gone;
            }
        }

        void OnItemClicked(object sender, TripClickedEventArgs e)
        {
            Intent detailsIntent = new Intent(this, typeof(TripDetailsActivity));
            detailsIntent.PutExtra("routeInfos", JsonConvert.SerializeObject(routeInfo));
            detailsIntent.PutExtra("tripInfos", JsonConvert.SerializeObject(e.Trip));

            StartActivity(detailsIntent);
        }

        void UpdateEmptyMessage(RouteDetails routeDetails)
        {
            if (routeDetails == null || routeDetails.trips.Count == 0)
            {
                viewPager.Visibility = ViewStates.Gone;

                if (Dependency.NetworkStatusMonitor.CanConnect)
                {
                    emptyView.Visibility = ViewStates.Visible;
                    emptyViewNoInternet.Visibility = ViewStates.Gone;
                }
                else
                {
                    emptyView.Visibility = ViewStates.Gone;
                    emptyViewNoInternet.Visibility = ViewStates.Visible;
                }
            }
            else
            {
                viewPager.Visibility = ViewStates.Visible;
                emptyView.Visibility = ViewStates.Gone;
                emptyViewNoInternet.Visibility = ViewStates.Gone;
            }
        }

        protected override async void OnSelectedDateChanged(object sender, DateTime selectedDate)
        {
            emptyViewMainText.Text = string.Format(Resources.GetText(Resource.String.trip_list_empty), TimeFormatter.ToAbrevShortDate(selectedDate));
            await LoadDetails();
        }
    }
}

