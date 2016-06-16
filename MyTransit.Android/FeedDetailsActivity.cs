﻿
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
using Newtonsoft.Json;
using MyTransit.Core;
using MyTransit.Android;
using Android.Support.V7.App;
using ToolbarCompat = Android.Support.V7.Widget.Toolbar;
using SearchViewCompat = Android.Support.V7.Widget.SearchView;
using MyTransit.Android.Adapters;
using MyTransit.Core.DataAccessor;
using Android.Support.V4.Widget;
using System.Threading.Tasks;

namespace MyTransit.Android
{
	[Activity(Label = "FeedDetailsActivity", ParentActivity = typeof(MainActivity))]
	public class FeedDetailsActivity : AppCompatActivity
	{
		private Feed feedInfo;
		private RouteAdapter routeAdapter;
		private ListView routeListView;
		private SwipeRefreshLayout routePullToRefresh;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.feed_details);

			var toolbar = FindViewById<ToolbarCompat>(Resource.Id.my_awesome_toolbar);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);

			routeListView = FindViewById<ListView>(Resource.Id.route_listview);
			routePullToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.route_pull_to_refresh);

			feedInfo = JsonConvert.DeserializeObject<Feed>(Intent.GetStringExtra("feedInfos"));
			this.Title = feedInfo.agency_name;

			routePullToRefresh.SetColorSchemeResources(Resource.Color.refresh_progress_1, Resource.Color.refresh_progress_2, Resource.Color.refresh_progress_3);
			routePullToRefresh.Post(async () =>
			{
				await LoadRoutes();
			});
			routePullToRefresh.Refresh += async delegate
			{
				await LoadRoutes();
			};
		}

		private async Task LoadRoutes()
		{
			routePullToRefresh.Refreshing = true;
			var routes = await RouteAccessor.GetAllRoutes(feedInfo.feed_id);

			if (routeAdapter == null)
			{
				routeAdapter = new RouteAdapter(this, routes);
				routeListView.Adapter = routeAdapter;
			}
			else {
				routeAdapter.ReplaceItems(routes);
			}

			routePullToRefresh.Refreshing = false;
		}
	}
}

