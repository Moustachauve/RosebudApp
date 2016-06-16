
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

namespace MyTransit.Android
{
	[Activity(Label = "FeedDetailsActivity", ParentActivity = typeof(MainActivity))]
	public class FeedDetailsActivity : AppCompatActivity
	{
		private Feed feedInfo;
		private RouteAdapter routeAdapter;
		private ListView routeListView;
		private ProgressBar routeProgressBar;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.feed_details);

			var toolbar = FindViewById<ToolbarCompat>(Resource.Id.my_awesome_toolbar);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);

			routeListView = FindViewById<ListView>(Resource.Id.route_listview);
			routeProgressBar = FindViewById<ProgressBar>(Resource.Id.route_progress_bar);

			feedInfo = JsonConvert.DeserializeObject<Feed>(Intent.GetStringExtra("feedInfos"));
			this.Title = feedInfo.agency_name;

			LoadRoutes();
		}

		private async void LoadRoutes()
		{
			routeProgressBar.Visibility = ViewStates.Visible;
			var routes = await RouteAccessor.GetAllRoutes(feedInfo.feed_id);

			if (routeAdapter == null)
			{
				routeAdapter = new RouteAdapter(this, routes);
				routeListView.Adapter = routeAdapter;
			}
			else {
				routeAdapter.ReplaceItems(routes);
			}

			routeProgressBar.Visibility = ViewStates.Gone;
		}
	}
}

