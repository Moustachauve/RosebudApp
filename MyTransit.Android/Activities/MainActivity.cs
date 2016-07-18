using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using MyTransit.Android.Adapters;
using MyTransit.Core.DataAccessor;
using MyTransit.Core.Model;
using Newtonsoft.Json;
using SearchViewCompat = Android.Support.V7.Widget.SearchView;
using ToolbarCompat = Android.Support.V7.Widget.Toolbar;

namespace MyTransit.Android.Activities
{
	[Activity(Label = "MyTransit", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : AppCompatActivity
	{
		private FeedAdapter feedAdapter;
		private RecyclerView feedRecyclerView;
		private SwipeRefreshLayout feedPullToRefresh;
		private IMenuItem searchMenu;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			HttpHelper.CacheRepository = new CacheRepository(ApplicationContext);

			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);

			var toolbar = FindViewById<ToolbarCompat>(Resource.Id.my_awesome_toolbar);
			SetSupportActionBar(toolbar);

			feedRecyclerView = FindViewById<RecyclerView>(Resource.Id.feed_recyclerview);
			feedPullToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.feed_pull_to_refresh);

			feedPullToRefresh.SetColorSchemeResources(Resource.Color.refresh_progress_1, Resource.Color.refresh_progress_2, Resource.Color.refresh_progress_3);
			feedPullToRefresh.Post(async () => {
				await LoadFeeds();
			});

			feedPullToRefresh.Refresh += async delegate
			{
				await LoadFeeds(true);
			};
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.menu_main, menu);

			searchMenu = menu.FindItem(Resource.Id.action_search);
			searchMenu.SetVisible(feedAdapter != null);

			var searchViewJava = MenuItemCompat.GetActionView(searchMenu);
			SearchViewCompat searchView = searchViewJava.JavaCast<SearchViewCompat>();

			searchView.QueryTextChange += (sender, args) =>
			{
				feedAdapter.Filter = args.NewText;
			};

			return true;
		}

		private async Task LoadFeeds(bool overrideCache = false)
		{
			feedPullToRefresh.Refreshing = true;
			var feeds = await FeedAccessor.GetAllFeeds(overrideCache);

			if (feedAdapter == null)
			{
				feedAdapter = new FeedAdapter(this, feeds);
				feedAdapter.ItemClick += OnItemClick;
				feedRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
				feedRecyclerView.SetAdapter(feedAdapter);
				InvalidateOptionsMenu();
			}
			else
				feedAdapter.ReplaceItems(feeds);

			feedPullToRefresh.Refreshing = false;
		}

		private void OnItemClick(object sender, int position)
		{
			Feed clickedFeed = feedAdapter[position];
			Intent detailsIntent = new Intent(this, typeof(FeedDetailsActivity));
			detailsIntent.PutExtra("feedInfos", JsonConvert.SerializeObject(clickedFeed));

			StartActivity(detailsIntent);
		}
	}
}


