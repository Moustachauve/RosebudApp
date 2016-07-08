
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
using MyTransit.Android.Adapters;
using MyTransit.Core;
using MyTransit.Core.DataAccessor;
using MyTransit.Core.Model;
using Newtonsoft.Json;
using SearchViewCompat = Android.Support.V7.Widget.SearchView;
using ToolbarCompat = Android.Support.V7.Widget.Toolbar;

namespace MyTransit.Android.Activities
{
	[Activity(Label = "FeedDetailsActivity", ParentActivity = typeof(MainActivity))]
	public class FeedDetailsActivity : AppCompatActivity
	{
		private Feed feedInfo;

		private AppBarLayout appBarLayout;
		private TextView lblToolbarDate;
		private RouteAdapter routeAdapter;
		private RecyclerView routeRecyclerView;
		private SwipeRefreshLayout routePullToRefresh;
		private IMenuItem searchMenu;

		private ImageView icoDropdownDatePicker;
		private DateTime currentDate;
		private bool isCalendarExpanded;
		private float currentCalendarArrowRotation = 360f;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.feed_details);

			appBarLayout = FindViewById<AppBarLayout>(Resource.Id.app_bar_layout);
			var toolbar = FindViewById<ToolbarCompat>(Resource.Id.my_awesome_toolbar);
			var lblToolbarTitle = FindViewById<TextView>(Resource.Id.lbl_toolbar_title);
			lblToolbarDate = FindViewById<TextView>(Resource.Id.lbl_toolbar_date);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);

			routeRecyclerView = FindViewById<RecyclerView>(Resource.Id.route_recyclerview);
			routeRecyclerView.NestedScrollingEnabled = false;
			routePullToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.route_pull_to_refresh);

			var btnDatePicker = FindViewById<RelativeLayout>(Resource.Id.btn_date_picker);
			icoDropdownDatePicker = FindViewById<ImageView>(Resource.Id.ico_dropdown_calendar);
			var calendarView = FindViewById<CalendarView>(Resource.Id.calendar_view);

			feedInfo = JsonConvert.DeserializeObject<Feed>(Intent.GetStringExtra("feedInfos"));
			lblToolbarTitle.Text = feedInfo.agency_name;

			routePullToRefresh.SetColorSchemeResources(Resource.Color.refresh_progress_1, Resource.Color.refresh_progress_2, Resource.Color.refresh_progress_3);
			routePullToRefresh.Post(async () =>
			{
				await SwitchDate(DateTime.Today);
			});

			routePullToRefresh.Refresh += async delegate
			{
				await LoadRoutes();
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

		private async Task SwitchDate(int year, int month, int day) {
			await SwitchDate(new DateTime(year, month, day));
		}

		private async Task SwitchDate(DateTime date)
		{
			currentDate = date;
			lblToolbarDate.Text = TimeFormatter.ToFullShortDate(date);
			if (routeAdapter != null)
				routeAdapter.ClearItems();
			await LoadRoutes();
		}

		private async Task LoadRoutes()
		{
			routePullToRefresh.Refreshing = true;
			var routes = await RouteAccessor.GetAllRoutes(feedInfo.feed_id);

			if (routeAdapter == null)
			{
				routeAdapter = new RouteAdapter(this, routes);
				routeAdapter.ItemClick += OnItemClick;
				routeRecyclerView.SetAdapter(routeAdapter);
				routeRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
				InvalidateOptionsMenu();
			}
			else {
				routeAdapter.ReplaceItems(routes);
			}

			routePullToRefresh.Refreshing = false;
		}

		public void OnItemClick(object sender, int position)
		{
			Route clickedRoute = routeAdapter[position];
			Intent detailsIntent = new Intent(this, typeof(RouteDetailsActivity));
			detailsIntent.PutExtra("routeInfos", JsonConvert.SerializeObject(clickedRoute));

			StartActivity(detailsIntent);
		}

		private void ToggleDatePicker() {
			RotateAnimation animation = ArrowRotateAnimation.GetAnimation(currentCalendarArrowRotation, 180);
			icoDropdownDatePicker.StartAnimation(animation);

			currentCalendarArrowRotation = (currentCalendarArrowRotation + 180f) % 360f;

			appBarLayout.SetExpanded(!isCalendarExpanded, true);
			isCalendarExpanded = !isCalendarExpanded;
		}
	}
}
