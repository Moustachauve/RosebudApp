
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

namespace RosebudAppAndroid.Activities
{
    [Activity(Label = "RouteDetailsActivity"/*, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize*/)]
    public class RouteDetailsActivity : AppCompatActivity
    {
        const string BUNDLE_KEY_DATE = "bundle_route_details_date";

        private Route routeInfo;
        private TripDirectionPagerAdapter tripDirectionPagerAdapter;

        private AppBarLayout appBarLayout;
        private TextView lblToolbarDate;
        private TabLayout tabLayout;

        private ImageView icoDropdownDatePicker;
        private bool isCalendarExpanded;
        private float currentCalendarArrowRotation = 360f;

        private ViewPager viewPager;
        private LinearLayout emptyView;
        private LinearLayout emptyViewNoInternet;
        private TextView emptyViewMainText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.route_details);

            appBarLayout = FindViewById<AppBarLayout>(Resource.Id.app_bar_layout);
            var toolbar = FindViewById<ToolbarCompat>(Resource.Id.my_awesome_toolbar);
            var lblToolbarTitle = FindViewById<TextView>(Resource.Id.lbl_toolbar_title);
            lblToolbarDate = FindViewById<TextView>(Resource.Id.lbl_toolbar_date);
            NetworkStatusFragment networkFragment = (NetworkStatusFragment)SupportFragmentManager.FindFragmentById(Resource.Id.network_fragment);

            tabLayout = FindViewById<TabLayout>(Resource.Id.tab_layout);
            viewPager = FindViewById<ViewPager>(Resource.Id.view_pager);
            emptyView = FindViewById<LinearLayout>(Resource.Id.empty_view);
            emptyViewNoInternet = FindViewById<LinearLayout>(Resource.Id.empty_view_no_internet);
            emptyViewMainText = FindViewById<TextView>(Resource.Id.empty_view_main_text);

            var btnDatePicker = FindViewById<RelativeLayout>(Resource.Id.btn_date_picker);
            icoDropdownDatePicker = FindViewById<ImageView>(Resource.Id.ico_dropdown_calendar);
            var calendarView = FindViewById<CalendarView>(Resource.Id.calendar_view);

            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            routeInfo = JsonConvert.DeserializeObject<Route>(Intent.GetStringExtra("routeInfos"));
            lblToolbarTitle.Text = routeInfo.route_short_name + " " + routeInfo.route_long_name;

            toolbar.NavigationClick += delegate
            {
                OnBackPressed();
            };

            btnDatePicker.Click += delegate
            {
                ToggleDatePicker();
            };

            calendarView.DateChange += async (object sender, CalendarView.DateChangeEventArgs e) =>
            {
                ToggleDatePicker();
                await SwitchDate(e.Year, e.Month + 1, e.DayOfMonth);
            };

            calendarView.Post(async () =>
            {
                long epochTime = (long)(Dependency.PreferenceManager.SelectedDatetime.AddDays(1) - new DateTime(1970, 1, 1)).TotalMilliseconds;
                calendarView.SetDate(epochTime, false, true);
                await SwitchDate(Dependency.PreferenceManager.SelectedDatetime);
            });

            networkFragment.RetryLastRequest += async (object sender, EventArgs args) =>
            {
                await LoadDetails();
            };
        }

        protected override void OnDestroy()
        {
            if (tripDirectionPagerAdapter != null)
            {
                tripDirectionPagerAdapter.ItemClicked -= OnItemClicked;
            }

            base.OnDestroy();
        }

        private async Task SwitchDate(int year, int month, int day)
        {
            await SwitchDate(new DateTime(year, month, day));
        }

        private async Task SwitchDate(DateTime date)
        {
            Dependency.PreferenceManager.SelectedDatetime = date;
            lblToolbarDate.Text = TimeFormatter.ToFullShortDate(date);
            emptyViewMainText.Text = string.Format(Resources.GetText(Resource.String.trip_list_empty), TimeFormatter.ToAbrevShortDate(date));
            await LoadDetails();
        }

        private async Task LoadDetails(bool overrideCache = false)
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

        private void ToggleDatePicker()
        {
            RotateAnimation animation = ArrowRotateAnimation.GetAnimation(currentCalendarArrowRotation, 180);
            icoDropdownDatePicker.StartAnimation(animation);

            currentCalendarArrowRotation = (currentCalendarArrowRotation + 180f) % 360f;

            appBarLayout.SetExpanded(!isCalendarExpanded, true);
            isCalendarExpanded = !isCalendarExpanded;
        }

        private void SetDirectionTabs(RouteDetails routeDetails)
        {
            tabLayout.RemoveAllTabs();

            if (routeDetails != null)
            {
                if (routeDetails.HasMultipleDirection())
                {
                    tabLayout.Visibility = ViewStates.Visible;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(routeDetails.GetDirectionName(TripDirection.AnyDirection)))
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

        private void OnItemClicked(object sender, TripClickedEventArgs e)
        {
            Intent detailsIntent = new Intent(this, typeof(TripDetailsActivity));
            detailsIntent.PutExtra("routeInfos", JsonConvert.SerializeObject(routeInfo));
            detailsIntent.PutExtra("tripInfos", JsonConvert.SerializeObject(e.Trip));

            StartActivity(detailsIntent);
        }

        private void UpdateEmptyMessage(RouteDetails routeDetails)
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
    }
}

