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
        FeedAdapter feedAdapter;
        RecyclerView feedRecyclerView;
        SwipeRefreshLayout feedPullToRefresh;
        SwipeRefreshLayout feedPullToRefreshEmpty;

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

            feedPullToRefresh.SetColorSchemeResources(Resource.Color.refresh_progress_1, Resource.Color.refresh_progress_2, Resource.Color.refresh_progress_3);
            feedPullToRefreshEmpty.SetColorSchemeResources(Resource.Color.refresh_progress_1, Resource.Color.refresh_progress_2, Resource.Color.refresh_progress_3);
            feedPullToRefresh.Post(async () =>
            {
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

            return view;
        }

        public override void OnAttach(Context context)
        {
            ((MainActivity)Activity).SearchTextChanged += OnSearchTextChanged;
            ((MainActivity)Activity).RetryLastRequest += OnRetryLastRequest;
            base.OnAttach(context);
        }

        public override void OnDetach()
        {
            ((MainActivity)Activity).SearchTextChanged -= OnSearchTextChanged;
            ((MainActivity)Activity).RetryLastRequest += OnRetryLastRequest;
            base.OnDetach();
        }

        async Task LoadFeeds(bool overrideCache = false)
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
            }
            else
                feedAdapter.ReplaceItems(feeds);

            ((MainActivity)Activity).SearchBarVisible = feedPullToRefresh.Visibility == ViewStates.Visible;


            feedPullToRefresh.Refreshing = false;
            feedPullToRefreshEmpty.Refreshing = false;
        }

        void OnItemClick(object sender, int position)
        {
            Feed clickedFeed = feedAdapter[position];
            Intent detailsIntent = new Intent(Activity, typeof(FeedDetailsActivity));
            detailsIntent.PutExtra("feedInfos", JsonConvert.SerializeObject(clickedFeed));

            StartActivity(detailsIntent);
        }

        void OnSearchTextChanged(object sender, string e)
        {
            feedAdapter.Filter = e;
        }

        async void OnRetryLastRequest(object sender, EventArgs args)
        {
            await LoadFeeds(true);
        }
    }
}