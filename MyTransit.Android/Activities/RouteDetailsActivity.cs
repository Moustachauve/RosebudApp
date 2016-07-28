
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
using MyTransit.Android.Fragments;
using MyTransit.Core;
using MyTransit.Core.DataAccessor;
using MyTransit.Core.Model;
using Newtonsoft.Json;
using ToolbarCompat = Android.Support.V7.Widget.Toolbar;

namespace MyTransit.Android.Activities
{
    [Activity(Label = "RouteDetailsActivity")]
    public class RouteDetailsActivity : AppCompatActivity
    {
        private Route routeInfo;
        private TripDirectionPagerAdapter tripDirectionPagerAdapter;

        private AppBarLayout appBarLayout;
        private TextView lblToolbarDate;
        private TabLayout tabLayout;

        private ImageView icoDropdownDatePicker;
        private DateTime currentDate;
        private bool isCalendarExpanded;
        private float currentCalendarArrowRotation = 360f;

        private ViewPager viewPager;
        private TextView emptyView;

        private RouteDetails currentRouteDetails;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.route_details);

            appBarLayout = FindViewById<AppBarLayout>(Resource.Id.app_bar_layout);
            var toolbar = FindViewById<ToolbarCompat>(Resource.Id.my_awesome_toolbar);
            var lblToolbarTitle = FindViewById<TextView>(Resource.Id.lbl_toolbar_title);
            lblToolbarDate = FindViewById<TextView>(Resource.Id.lbl_toolbar_date);

            tabLayout = FindViewById<TabLayout>(Resource.Id.tab_layout);
            viewPager = FindViewById<ViewPager>(Resource.Id.view_pager);
            emptyView = FindViewById<TextView>(Resource.Id.empty_view);

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

            viewPager.Post(async () =>
            {
                await SwitchDate(DateTime.Today);
            });
        }

        private async Task SwitchDate(int year, int month, int day)
        {
            await SwitchDate(new DateTime(year, month, day));
        }

        private async Task SwitchDate(DateTime date)
        {
            currentDate = date;
            lblToolbarDate.Text = TimeFormatter.ToFullShortDate(date);
            await LoadDetails();
        }

        private async Task LoadDetails(bool overrideCache = false)
        {
            currentRouteDetails = await RouteAccessor.GetRouteDetails(routeInfo.feed_id, routeInfo.route_id, currentDate, overrideCache);

            viewPager.Visibility = currentRouteDetails == null ? ViewStates.Gone : ViewStates.Visible;
            emptyView.Visibility = currentRouteDetails == null ? ViewStates.Visible : ViewStates.Gone;

            SetDirectionTabs();

            tripDirectionPagerAdapter = new TripDirectionPagerAdapter(SupportFragmentManager, currentRouteDetails);
            tripDirectionPagerAdapter.ItemClicked += OnItemClicked;
            viewPager.Adapter = tripDirectionPagerAdapter;
            tabLayout.SetupWithViewPager(viewPager);

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

        private void SetDirectionTabs()
        {
            tabLayout.RemoveAllTabs();

            if (currentRouteDetails != null)
            {
                if (currentRouteDetails.HasMultipleDirection())
                {
                    tabLayout.Visibility = ViewStates.Visible;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(currentRouteDetails.GetDirectionName(TripDirection.AnyDirection)))
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
    }
}

