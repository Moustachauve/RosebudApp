using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using ToolbarCompat = Android.Support.V7.Widget.Toolbar;
using RosebudAppAndroid.Fragments;
using Android.Support.V4.View;
using Newtonsoft.Json;
using RosebudAppCore.Utils;
using RosebudAppCore.Model;
using Android.Views.Animations;
using System.Threading.Tasks;
using RosebudAppCore;
using RosebudAppCore.DataAccessor;
using RosebudAppCore.Model.Enum;
using RosebudApp.AndroidMaterialCalendarBinding;

namespace RosebudAppAndroid.Activities
{
    [Activity(Label = "StopSelectionActivity")]
    public class StopSelectionActivity : CalendarToolBarActivity
    {
        const string STATE_TAB_PAGE = "state-tab-page";
        const string STATE_RECYCLER_VIEW = "state-recycler-view";

        Route routeInfo;
        StopDirectionPagerAdapter stopDirectionPagerAdapter;
        TabLayout tabLayout;

        ViewPager viewPager;
        LinearLayout emptyView;
        LinearLayout emptyViewNoInternet;
        TextView emptyViewMainText;

        int? restoreTabPage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            ActivityLayout = Resource.Layout.stop_selection;
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
                await LoadStops();
            };

            SwitchDate(Dependency.PreferenceManager.SelectedDatetime);
        }


        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutInt(STATE_TAB_PAGE, tabLayout.SelectedTabPosition);
        }
        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            restoreTabPage = savedInstanceState.GetInt(STATE_TAB_PAGE);
        }

        async Task LoadStops(bool overrideCache = false)
        {
            List<Stop> currentStops = await RouteAccessor.GetRouteStops(routeInfo.feed_id, routeInfo.route_id, Dependency.PreferenceManager.SelectedDatetime, overrideCache);

            UpdateEmptyMessage(currentStops);
            SetDirectionTabs(currentStops);

            if (stopDirectionPagerAdapter == null)
            {
                stopDirectionPagerAdapter = new StopDirectionPagerAdapter(SupportFragmentManager, routeInfo);
                stopDirectionPagerAdapter.ItemClicked += OnItemClicked;
                viewPager.Adapter = stopDirectionPagerAdapter;
                tabLayout.SetupWithViewPager(viewPager);
            }

            stopDirectionPagerAdapter.UpdateStops(currentStops);
            tabLayout.GetTabAt(restoreTabPage ?? 0).Select();

            InvalidateOptionsMenu();
        }

        void UpdateEmptyMessage(List<Stop> currentStops)
        {
            if (currentStops == null || currentStops.Count == 0)
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

        void SetDirectionTabs(List<Stop> currentStops)
        {
            tabLayout.RemoveAllTabs();

            if (currentStops != null)
            {
                if (TripDirectionHelper.HasMultipleDirection(currentStops))
                {
                    tabLayout.Visibility = ViewStates.Visible;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(TripDirectionHelper.GetDirectionName(currentStops, TripDirection.AnyDirection)))
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


        void OnItemClicked(object sender, StopClickedEventArgs e)
        {
            Intent detailsIntent = new Intent(this, typeof(StopTimeActivity));
            detailsIntent.PutExtra("routeInfos", JsonConvert.SerializeObject(routeInfo));
            detailsIntent.PutExtra("stopInfos", JsonConvert.SerializeObject(e.Stop));

            StartActivity(detailsIntent);
        }

        protected override async void OnSelectedDateChanged(object sender, DateTime selectedDate)
        {
            emptyViewMainText.Text = string.Format(Resources.GetText(Resource.String.trip_list_empty), TimeFormatter.ToAbrevShortDate(selectedDate));
            await LoadStops();
        }
    }
}