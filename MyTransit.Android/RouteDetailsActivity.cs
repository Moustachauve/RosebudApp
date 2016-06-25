
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

namespace MyTransit.Android
{
	[Activity(Label = "RouteDetailsActivity")]
	public class RouteDetailsActivity : AppCompatActivity
	{
		private Route routeInfo;
		private TripAdapter tripAdapter;
		private ListView tripListView;
		private SwipeRefreshLayout routePullToRefresh;
		private IMenuItem searchMenu;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.feed_details);

			var toolbar = FindViewById<ToolbarCompat>(Resource.Id.my_awesome_toolbar);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);

			toolbar.NavigationClick += delegate
			{
				OnBackPressed();
			};

			tripListView = FindViewById<ListView>(Resource.Id.route_listview);
			routePullToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.route_pull_to_refresh);

			routeInfo = JsonConvert.DeserializeObject<Route>(Intent.GetStringExtra("routeInfos"));
			this.Title = routeInfo.route_short_name + " " + routeInfo.route_long_name;

			routePullToRefresh.SetColorSchemeResources(Resource.Color.refresh_progress_1, Resource.Color.refresh_progress_2, Resource.Color.refresh_progress_3);
			routePullToRefresh.Post(async () =>
			{
				await LoadRoutes();
			});
			routePullToRefresh.Refresh += async delegate
			{
				await LoadRoutes();
			};
			tripListView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
			{
				Trip clickedTrip = tripAdapter[args.Position];
				Intent detailsIntent = new Intent(this, typeof(TripDetailsActivity));
				detailsIntent.PutExtra("routeInfos", JsonConvert.SerializeObject(routeInfo));
				detailsIntent.PutExtra("tripInfos", JsonConvert.SerializeObject(clickedTrip));

				StartActivity(detailsIntent);
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

		private async Task LoadRoutes()
		{
			routePullToRefresh.Refreshing = true;
			var trips = await TripAccessor.GetTripsForRoute(routeInfo.feed_id, routeInfo.route_id);

			if (tripAdapter == null)
			{
				tripAdapter = new TripAdapter(this, trips);
				tripListView.Adapter = tripAdapter;
				InvalidateOptionsMenu();
			}
			else {
				tripAdapter.ReplaceItems(trips);
			}

			tripListView.Post(() =>
			{
				tripListView.SmoothScrollToPositionFromTop(tripAdapter.GetPositionOfNextTrip(), 50);
			});

			routePullToRefresh.Refreshing = false;
		}
	}
}

