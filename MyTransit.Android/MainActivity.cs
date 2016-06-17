using Android.App;
using Android.OS;
using MyTransit.Core;
using Android.Views;
using Android.Content;
using Newtonsoft.Json;
using Android.Support.V7.App;
using Android.Widget;
using ToolbarCompat = Android.Support.V7.Widget.Toolbar;
using SearchViewCompat = Android.Support.V7.Widget.SearchView;
using System;
using Android.Support.V4.View;
using Android.Runtime;
using Android.Support.V7.View.Menu;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using System.Threading.Tasks;
using MyTransit.Android.Adapters;
using MyTransit.Core.DataAccessor;
using MyTransit.Core.Model;

namespace MyTransit.Android
{
	[Activity(Label = "MyTransit", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : AppCompatActivity
	{
		private FeedAdapter feedAdapter;
		private ListView feedListView;
		private SwipeRefreshLayout feedPullToRefresh;
		private IMenuItem searchMenu;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);

			var toolbar = FindViewById<ToolbarCompat>(Resource.Id.my_awesome_toolbar);
			SetSupportActionBar(toolbar);

			feedListView = FindViewById<ListView>(Resource.Id.feed_listview);
			feedPullToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.feed_pull_to_refresh);

			feedListView.TextFilterEnabled = true;

			feedPullToRefresh.SetColorSchemeResources(Resource.Color.refresh_progress_1, Resource.Color.refresh_progress_2, Resource.Color.refresh_progress_3);
			feedPullToRefresh.Post(async () => {
				await LoadFeeds();
			});

			feedListView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
			{
				Feed clickedFeed = feedAdapter[args.Position];
				Intent detailsIntent = new Intent(this, typeof(FeedDetailsActivity));
				detailsIntent.PutExtra("feedInfos", JsonConvert.SerializeObject(clickedFeed));

				StartActivity(detailsIntent);
			};

			feedPullToRefresh.Refresh += async delegate
			{
				await LoadFeeds();
			};
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.menu_main, menu);

			searchMenu = menu.FindItem(Resource.Id.action_search);
			searchMenu.SetVisible(feedAdapter != null);

			var searchViewJava = MenuItemCompat.GetActionView(searchMenu);
			SearchViewCompat searchView = searchViewJava.JavaCast<SearchViewCompat>();
			//searchView.QueryTextSubmit

			searchView.QueryTextChange += (sender, args) =>
			{
				feedAdapter.Filter = args.NewText;
			};

			return true;
		}

		private async Task LoadFeeds()
		{
			feedPullToRefresh.Refreshing = true;
			var feeds = await FeedAccessor.GetAllFeeds();

			if (feedAdapter == null)
			{
				feedAdapter = new FeedAdapter(this, feeds);
				feedListView.Adapter = feedAdapter;
				InvalidateOptionsMenu();
			}
			else
				feedAdapter.ReplaceItems(feeds);

			feedPullToRefresh.Refreshing = false;
		}
	}
}


