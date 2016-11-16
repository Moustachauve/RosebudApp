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
using RosebudAppAndroid.Views;
using Android.Support.V4.View;
using FragmentSupport = Android.Support.V4.App.Fragment;

namespace RosebudAppAndroid.Fragments
{
    public class FeedListFragment : FragmentSupport
    {
        const string STATE_RECYCLER_VIEW = "state-recycler-view";

        FeedAdapter feedAdapter;
        RecyclerView feedRecyclerView;
        LoadingContainer loadingContainer;
        View emptyView;

        IParcelable recyclerViewLayoutState;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.feed_list, container, false);

            feedRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.feed_recyclerview);
            loadingContainer = view.FindViewById<LoadingContainer>(Resource.Id.loading_container);
            emptyView = view.FindViewById<View>(Resource.Id.empty_view);

            //This line enable smooth scrolling inside the LoadingContainer
            ViewCompat.SetNestedScrollingEnabled(feedRecyclerView, false);

            loadingContainer.Post(async () =>
            {
                loadingContainer.Loading = true;
                await LoadFeeds();
                loadingContainer.Loading = false;
            });

            loadingContainer.Refresh += async delegate
            {
                loadingContainer.Refreshing = true;
                await LoadFeeds(true);
                loadingContainer.Refreshing = false;
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
            ((MainActivity)Activity).RetryLastRequest -= OnRetryLastRequest;
            base.OnDetach();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            if(feedRecyclerView != null && feedRecyclerView.GetLayoutManager() != null)
            {
                outState.PutParcelable(STATE_RECYCLER_VIEW, feedRecyclerView.GetLayoutManager().OnSaveInstanceState());
            }
        }

        public override void OnViewStateRestored(Bundle savedInstanceState)
        {
            base.OnViewStateRestored(savedInstanceState);
            if (savedInstanceState != null)
            {
                if(savedInstanceState.ContainsKey(STATE_RECYCLER_VIEW))
                    recyclerViewLayoutState = (IParcelable)savedInstanceState.GetParcelable(STATE_RECYCLER_VIEW);
            }
        }

        async Task LoadFeeds(bool overrideCache = false)
        {
            var feeds = await FeedAccessor.GetAllFeeds(overrideCache);

            feedRecyclerView.Visibility = feeds == null ? ViewStates.Gone : ViewStates.Visible;
            emptyView.Visibility = feeds == null ? ViewStates.Visible : ViewStates.Gone;

            if (feedAdapter == null)
            {
                feedAdapter = new FeedAdapter(Activity, feeds);
                feedAdapter.ItemClick += OnItemClick;
                feedRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
                feedRecyclerView.SetAdapter(feedAdapter);

                if (recyclerViewLayoutState != null)
                {
                    feedRecyclerView.GetLayoutManager().OnRestoreInstanceState(recyclerViewLayoutState);
                }
            }
            else
                feedAdapter.ReplaceItems(feeds);

            ((MainActivity)Activity).SearchBarVisible = feeds != null;
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