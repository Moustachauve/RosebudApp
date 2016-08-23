using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using RosebudAppAndroid.Adapters;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using System.Threading.Tasks;
using RosebudAppCore.DataAccessor;
using RosebudAppCore.Model;
using RosebudAppAndroid.Activities;
using Newtonsoft.Json;
using Android.Support.V7.App;
using Android.App;

namespace RosebudAppAndroid.Fragments
{
    public class FeedListFragment : Fragment
    {
        private FeedAdapter feedAdapter;
        private RecyclerView feedRecyclerView;
        private SwipeRefreshLayout feedPullToRefresh;
        private SwipeRefreshLayout feedPullToRefreshEmpty;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.feed_list, container, false);

            feedRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.feed_recyclerview);
            feedPullToRefresh = view.FindViewById<SwipeRefreshLayout>(Resource.Id.feed_pull_to_refresh);
            feedPullToRefreshEmpty = view.FindViewById<SwipeRefreshLayout>(Resource.Id.feed_pull_to_refresh_empty);
            NetworkStatusFragment networkFragment = (NetworkStatusFragment)FragmentManager.FindFragmentById(Resource.Id.network_fragment);

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

            /*networkFragment.RetryLastRequest += async (object sender, EventArgs args) =>
            {
                await LoadFeeds(true);
            };*/

            return view;
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
                feedAdapter = new FeedAdapter(Activity, feeds);
                feedAdapter.ItemClick += OnItemClick;
                feedRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
                feedRecyclerView.SetAdapter(feedAdapter);
                //InvalidateOptionsMenu();
            }
            else
                feedAdapter.ReplaceItems(feeds);

            feedPullToRefresh.Refreshing = false;
            feedPullToRefreshEmpty.Refreshing = false;
        }

        private void OnItemClick(object sender, int position)
        {
            Feed clickedFeed = feedAdapter[position];
            Intent detailsIntent = new Intent(Activity, typeof(FeedDetailsActivity));
            detailsIntent.PutExtra("feedInfos", JsonConvert.SerializeObject(clickedFeed));

            StartActivity(detailsIntent);
        }

    }
}