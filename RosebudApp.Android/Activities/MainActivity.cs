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
using RosebudAppAndroid.Adapters;
using RosebudAppCore.DataAccessor;
using RosebudAppCore.Model;
using Newtonsoft.Json;
using SearchViewCompat = Android.Support.V7.Widget.SearchView;
using ToolbarCompat = Android.Support.V7.Widget.Toolbar;
using RosebudAppAndroid.Fragments;
using System;

namespace RosebudAppAndroid.Activities
{
	[Activity(Label = "Rosebud", MainLauncher = true, Icon = "@mipmap/ic_launcher")]
	public class MainActivity : AppCompatActivity
    {
		private FeedAdapter feedAdapter;
		private RecyclerView feedRecyclerView;
		private SwipeRefreshLayout feedPullToRefresh;
        private SwipeRefreshLayout feedPullToRefreshEmpty;
        private IMenuItem searchMenu;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);

			var toolbar = FindViewById<ToolbarCompat>(Resource.Id.my_awesome_toolbar);
			SetSupportActionBar(toolbar);

			feedRecyclerView = FindViewById<RecyclerView>(Resource.Id.feed_recyclerview);
			feedPullToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.feed_pull_to_refresh);
            feedPullToRefreshEmpty = FindViewById<SwipeRefreshLayout>(Resource.Id.feed_pull_to_refresh_empty);
            NetworkStatusFragment networkFragment = (NetworkStatusFragment)SupportFragmentManager.FindFragmentById(Resource.Id.network_fragment);

            feedPullToRefresh.SetColorSchemeResources(Resource.Color.refresh_progress_1, Resource.Color.refresh_progress_2, Resource.Color.refresh_progress_3);
            feedPullToRefreshEmpty.SetColorSchemeResources(Resource.Color.refresh_progress_1, Resource.Color.refresh_progress_2, Resource.Color.refresh_progress_3);
            feedPullToRefresh.Post(async () => {
				await LoadFeeds();
			});

            feedPullToRefresh.Refresh += async delegate
            {
                await LoadFeeds(true);
            };
            feedPullToRefreshEmpty.Refresh += async delegate
            {
                await LoadFeeds(true);
            };

            networkFragment.RetryLastRequest += async (object sender, EventArgs args) =>
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
            feedPullToRefreshEmpty.Refreshing = true;

            var feeds = await FeedAccessor.GetAllFeeds(overrideCache);

            feedPullToRefresh.Visibility = feeds == null ? ViewStates.Gone : ViewStates.Visible;
            feedPullToRefreshEmpty.Visibility = feeds == null ? ViewStates.Visible : ViewStates.Gone;

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
            feedPullToRefreshEmpty.Refreshing = false;
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


