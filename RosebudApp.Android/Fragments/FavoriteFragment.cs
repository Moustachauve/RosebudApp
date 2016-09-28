using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using RosebudAppAndroid.Adapters;
using Android.Support.V7.Widget;
using System.Threading.Tasks;
using RosebudAppCore.DataAccessor;
using RosebudAppAndroid.Activities;
using RosebudAppCore.Model;
using Newtonsoft.Json;

namespace RosebudAppAndroid.Fragments
{
    public class FavoriteFragment : Fragment
    {
        const string STATE_FEED_RECYCLER_VIEW = "state-feed-recycler-view";
        const string STATE_ROUTE_RECYCLER_VIEW = "state-route-recycler-view";

        FeedAdapter feedAdapter;
        RouteAdapter routeAdapter;
        RecyclerView feedRecyclerView;
        RecyclerView routeRecyclerView;
        TextView feedEmptyView;
        TextView routeEmptyView;

        IParcelable feedRecyclerViewLayoutState;
        IParcelable routeRecyclerViewLayoutState;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.favorite, container, false);

            feedRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.favorite_feed_recyclerview);
            routeRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.favorite_route_recyclerview);
            feedEmptyView = view.FindViewById<TextView>(Resource.Id.feed_list_empty);
            routeEmptyView = view.FindViewById<TextView>(Resource.Id.route_list_empty);

            feedRecyclerView.HasFixedSize = false;
            routeRecyclerView.HasFixedSize = false;

            feedRecyclerView.Post(async () =>
            {
                await LoadFavorites();
            });

            return view;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            if (feedRecyclerView != null && feedRecyclerView.GetLayoutManager() != null)
            {
                outState.PutParcelable(STATE_FEED_RECYCLER_VIEW, feedRecyclerView.GetLayoutManager().OnSaveInstanceState());
            }
            if (routeRecyclerView != null && routeRecyclerView.GetLayoutManager() != null)
            {
                outState.PutParcelable(STATE_ROUTE_RECYCLER_VIEW, routeRecyclerView.GetLayoutManager().OnSaveInstanceState());
            }
        }

        public override void OnViewStateRestored(Bundle savedInstanceState)
        {
            base.OnViewStateRestored(savedInstanceState);
            if (savedInstanceState != null)
            {
                if (savedInstanceState.ContainsKey(STATE_FEED_RECYCLER_VIEW))
                    feedRecyclerViewLayoutState = (IParcelable)savedInstanceState.GetParcelable(STATE_FEED_RECYCLER_VIEW);
                if (savedInstanceState.ContainsKey(STATE_ROUTE_RECYCLER_VIEW))
                    routeRecyclerViewLayoutState = (IParcelable)savedInstanceState.GetParcelable(STATE_ROUTE_RECYCLER_VIEW);
            }
        }

        async Task LoadFavorites()
        {
            await LoadFeeds();
            await LoadRoutes();

            ((MainActivity)Activity).SearchBarVisible = false;
        }

        async Task LoadFeeds()
        {
            var feeds = await FavoriteFeedAccessor.GetFavoriteFeeds();

            feedEmptyView.Visibility = feeds.Count > 0 ? ViewStates.Gone : ViewStates.Visible;
            feedRecyclerView.Visibility = feeds.Count > 0 ? ViewStates.Visible : ViewStates.Gone;

            if (feedAdapter == null)
            {
                feedAdapter = new FeedAdapter(Activity, feeds);
                feedAdapter.ItemClick += OnFeedItemClick;
                feedRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
                feedRecyclerView.SetAdapter(feedAdapter);

                if (feedRecyclerViewLayoutState != null)
                {
                    feedRecyclerView.GetLayoutManager().OnRestoreInstanceState(feedRecyclerViewLayoutState);
                }
            }
            else
                feedAdapter.ReplaceItems(feeds);

        }

        async Task LoadRoutes()
        {
            var routes = await FavoriteRouteAccessor.GetFavoriteRoutes();

            routeEmptyView.Visibility = routes.Count > 0 ? ViewStates.Gone : ViewStates.Visible;
            routeRecyclerView.Visibility = routes.Count > 0 ? ViewStates.Visible : ViewStates.Gone;

            if (routeAdapter == null)
            {
                routeAdapter = new RouteAdapter(Activity, routes);
                routeAdapter.ItemClick += OnRouteItemClick;
                routeRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
                routeRecyclerView.SetAdapter(routeAdapter);

                if (routeRecyclerViewLayoutState != null)
                {
                    routeRecyclerView.GetLayoutManager().OnRestoreInstanceState(routeRecyclerViewLayoutState);
                }
            }
            else
                routeAdapter.ReplaceItems(routes);

        }

        private void OnFeedItemClick(object sender, int e)
        {
            Feed clickedFeed = feedAdapter[e];
            Intent detailsIntent = new Intent(Activity, typeof(FeedDetailsActivity));
            detailsIntent.PutExtra("feedInfos", JsonConvert.SerializeObject(clickedFeed));

            StartActivity(detailsIntent);
        }

        private void OnRouteItemClick(object sender, int e)
        {
            Route clickedRoute = routeAdapter[e];

            Intent stopSelectionIntent = new Intent(Activity, typeof(StopSelectionActivity));
            stopSelectionIntent.PutExtra("routeInfos", JsonConvert.SerializeObject(clickedRoute));

            StartActivity(stopSelectionIntent);
        }
    }
}