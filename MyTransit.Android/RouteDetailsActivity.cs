
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
using MyTransit.Core.Model;
using MyTransit.Android.Adapters;
using Android.Support.V4.Widget;
using ToolbarCompat = Android.Support.V7.Widget.Toolbar;
using SearchViewCompat = Android.Support.V7.Widget.SearchView;
using Newtonsoft.Json;
using Android.Support.V4.View;
using System.Threading.Tasks;
using MyTransit.Core;
using Android.Support.V4.App;
using MyTransit.Core.DataAccessor;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using Android.Views.Animations;

namespace MyTransit.Android
{
	[Activity(Label = "RouteDetailsActivity")]
	public class RouteDetailsActivity : AppCompatActivity
	{
		private Route routeInfo;
		private TripAdapter tripAdapter;

		private AppBarLayout appBarLayout;
		private TextView lblToolbarDate;

		private RecyclerView tripRecyclerView;
		private SwipeRefreshLayout tripPullToRefresh;
		private IMenuItem searchMenu;

		private ImageView icoDropdownDatePicker;
		private DateTime currentDate;
		private bool isCalendarExpanded;
		private float currentCalendarArrowRotation = 360f;


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.route_details);

			appBarLayout = FindViewById<AppBarLayout>(Resource.Id.app_bar_layout);
			var toolbar = FindViewById<ToolbarCompat>(Resource.Id.my_awesome_toolbar);
			var lblToolbarTitle = FindViewById<TextView>(Resource.Id.lbl_toolbar_title);
			lblToolbarDate = FindViewById<TextView>(Resource.Id.lbl_toolbar_date);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);

			var btnDatePicker = FindViewById<RelativeLayout>(Resource.Id.btn_date_picker);
			icoDropdownDatePicker = FindViewById<ImageView>(Resource.Id.ico_dropdown_calendar);
			var calendarView = FindViewById<CalendarView>(Resource.Id.calendar_view);

			tripRecyclerView = FindViewById<RecyclerView>(Resource.Id.trip_recyclerview);
			tripRecyclerView.NestedScrollingEnabled = false;
			tripPullToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.trip_pull_to_refresh);

			routeInfo = JsonConvert.DeserializeObject<Route>(Intent.GetStringExtra("routeInfos"));
			lblToolbarTitle.Text = routeInfo.route_short_name + " " + routeInfo.route_long_name;

			tripPullToRefresh.SetColorSchemeResources(Resource.Color.refresh_progress_1, Resource.Color.refresh_progress_2, Resource.Color.refresh_progress_3);
			tripPullToRefresh.Post(async () =>
			{
				await SwitchDate(DateTime.Today);
			});

			tripPullToRefresh.Refresh += async delegate
			{
				await LoadTrips();
			};

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

		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.menu_main, menu);

			searchMenu = menu.FindItem(Resource.Id.action_search);
			searchMenu.SetVisible(tripAdapter != null);

			var searchViewJava = MenuItemCompat.GetActionView(searchMenu);
			SearchViewCompat searchView = searchViewJava.JavaCast<SearchViewCompat>();

			searchView.QueryTextChange += (sender, args) =>
			{
				tripAdapter.Filter = args.NewText;
			};

			return true;
		}

		private async Task SwitchDate(int year, int month, int day)
		{
			await SwitchDate(new DateTime(year, month, day));
		}

		private async Task SwitchDate(DateTime date)
		{
			currentDate = date;
			lblToolbarDate.Text = TimeFormatter.ToFullShortDate(date);
			if (tripAdapter != null)
				tripAdapter.ClearItems();
			await LoadTrips();
		}

		private async Task LoadTrips()
		{
			tripPullToRefresh.Refreshing = true;
			var trips = await TripAccessor.GetTripsForRoute(routeInfo.feed_id, routeInfo.route_id, currentDate);

			if (tripAdapter == null)
			{
				tripAdapter = new TripAdapter(this, trips);
				tripAdapter.ItemClick += OnItemClick;
				tripRecyclerView.SetAdapter(tripAdapter);
				tripRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
				InvalidateOptionsMenu();
			}
			else {
				tripAdapter.ReplaceItems(trips);
			}

			tripRecyclerView.Post(() =>
			{
				//TODO
				//tripRecyclerView.SmoothScrollToPositionFromTop(tripAdapter.GetPositionOfNextTrip(), 50);
			});

			tripPullToRefresh.Refreshing = false;
		}

		private void OnItemClick(object sender, int position) {
			Trip clickedTrip = tripAdapter[position];
			Intent detailsIntent = new Intent(this, typeof(TripDetailsActivity));
			detailsIntent.PutExtra("routeInfos", JsonConvert.SerializeObject(routeInfo));
			detailsIntent.PutExtra("tripInfos", JsonConvert.SerializeObject(clickedTrip));

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

