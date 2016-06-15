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

namespace MyTransit.Android
{
	[Activity(Label = "MyTransit", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : AppCompatActivity
	{
		private FeedAdapter feedAdapter;
		private ListView feedListView;
		private SwipeRefreshLayout feedPullToRefresh;
		private ProgressBar feedProgressBar;
		private IMenuItem searchMenu;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);

			var toolbar = FindViewById<ToolbarCompat>(Resource.Id.my_awesome_toolbar);
			SetSupportActionBar(toolbar);

			feedListView = FindViewById<ListView>(Resource.Id.feed_listview);
			feedPullToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.feed_pull_to_refresh);
			feedProgressBar = FindViewById<ProgressBar>(Resource.Id.feed_progress_bar);

			feedListView.TextFilterEnabled = true;

			LoadFeeds();

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
				feedPullToRefresh.Refreshing = false;
			};
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.menu_main, menu);

			searchMenu = menu.FindItem(Resource.Id.action_search);
			searchMenu.SetVisible(feedAdapter != null);
			//InvalidateOptionsMenu();

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
			feedProgressBar.Visibility = ViewStates.Visible;
			var feeds = await FeedAccessor.GetAllFeeds();

			if (feedAdapter == null)
			{
				feedAdapter = new FeedAdapter(this, feeds);
				feedListView.Adapter = feedAdapter;
				RunOnUiThread(() =>
				{
					InvalidateOptionsMenu();
				});
			}
			else
				feedAdapter.ReplaceItems(feeds);

			feedProgressBar.Visibility = ViewStates.Gone;
			feedPullToRefresh.Refreshing = false;
		}
	}
}


