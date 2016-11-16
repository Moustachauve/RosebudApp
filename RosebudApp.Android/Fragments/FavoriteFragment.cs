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
using RosebudAppCore.Utils;
using Android;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Gms.Common;
using RosebudAppAndroid.Utils;
using RosebudAppAndroid.Views;
using Android.Support.V4.View;
using FragmentSupport = Android.Support.V4.App.Fragment;
using Android.Support.V7.App;

namespace RosebudAppAndroid.Fragments
{
    public class FavoriteFragment : FragmentSupport, ILocationServiceListener
    {
        const string STATE_FEED_RECYCLER_VIEW = "state-feed-recycler-view";
        const string STATE_ROUTE_RECYCLER_VIEW = "state-route-recycler-view";
        const int REQUEST_GOOGLE_PLAY_SERVICES = 42;
        const int REQUEST_LOCATION_ID = 0;

        readonly string[] PermissionsLocation =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        LoadingContainer loadingContainer;
        FeedAdapter feedAdapter;
        FavoriteRouteAdapter routeAdapter;
        RecyclerView feedRecyclerView;
        RecyclerView routeRecyclerView;
        TextView feedEmptyView;
        TextView routeEmptyView;

        IParcelable feedRecyclerViewLayoutState;
        IParcelable routeRecyclerViewLayoutState;

        List<FavoriteRoute> favoriteRoutes;

        bool IsListeningToLocationChange;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.favorite, container, false);

            loadingContainer = view.FindViewById<LoadingContainer>(Resource.Id.loading_container);
            feedRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.favorite_feed_recyclerview);
            routeRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.favorite_route_recyclerview);
            feedEmptyView = view.FindViewById<TextView>(Resource.Id.feed_list_empty);
            routeEmptyView = view.FindViewById<TextView>(Resource.Id.route_list_empty);

            loadingContainer.Refreshable = false;
            feedRecyclerView.HasFixedSize = false;
            routeRecyclerView.HasFixedSize = false;
            //This line enable smooth scrolling inside the LoadingContainer
            ViewCompat.SetNestedScrollingEnabled(feedRecyclerView, false);
            ViewCompat.SetNestedScrollingEnabled(routeRecyclerView, false);

            feedRecyclerView.Post(async () =>
            {
                ((MainActivity)Activity).SearchBarVisible = false;
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

        public override void OnResume()
        {
            base.OnResume();

            if (IsListeningToLocationChange)
                Dependency.LocationService.AddOnLocationChangedListener(this);
        }

        public override void OnPause()
        {
            base.OnPause();

            if(IsListeningToLocationChange)
                Dependency.LocationService.RemoveOnLocationChangedListener(this);
        }

        async Task LoadFavorites()
        {
            loadingContainer.Loading = true;

            await LoadFeeds();
            await LoadRoutes();

            loadingContainer.Loading = false;

            await InitLocationListener();
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
            List<Route> routes = await FavoriteRouteAccessor.GetFavoriteRoutes();
            favoriteRoutes = new List<FavoriteRoute>();

            foreach (var route in routes)
            {
                FavoriteRoute routeWithStopLocation = new FavoriteRoute();
                routeWithStopLocation.Route = route;
                favoriteRoutes.Add(routeWithStopLocation);
            }

            routeEmptyView.Visibility = favoriteRoutes.Count > 0 ? ViewStates.Gone : ViewStates.Visible;
            routeRecyclerView.Visibility = favoriteRoutes.Count > 0 ? ViewStates.Visible : ViewStates.Gone;

            if (routeAdapter == null)
            {
                routeAdapter = new FavoriteRouteAdapter(Activity, favoriteRoutes, ChildFragmentManager);
                routeAdapter.ItemClick += OnRouteItemClick;
                routeRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
                routeRecyclerView.SetAdapter(routeAdapter);

                if (routeRecyclerViewLayoutState != null)
                {
                    routeRecyclerView.GetLayoutManager().OnRestoreInstanceState(routeRecyclerViewLayoutState);
                }
            }
            else
                routeAdapter.ReplaceItems(favoriteRoutes);
        }

        async Task InitLocationListener()
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                await StartListeningLocation();
            }
            else
            {
                await RequestLocationPermission();
            }
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
            Route clickedRoute = routeAdapter[e].Route;

            Intent stopSelectionIntent = new Intent(Activity, typeof(StopSelectionActivity));
            stopSelectionIntent.PutExtra("routeInfos", JsonConvert.SerializeObject(clickedRoute));

            StartActivity(stopSelectionIntent);
        }

        public async Task OnLocationChanged(Location location)
        {
            await UpdateRoutesLocation(location);
        }

        private async Task UpdateRoutesLocation(Location location)
        {
            if (favoriteRoutes == null || favoriteRoutes.Count == 0)
                return;

            foreach (var routeWithLocation in favoriteRoutes)
            {
                List<FavoriteRouteDirection> routeDirections = await RouteLocationHelper.GetClosestStops(routeWithLocation.Route, location.Latitude, location.Longitude);
                routeWithLocation.Directions = routeDirections;

                await RouteLocationHelper.GetNextTimeForFavoriteRoute(routeWithLocation);
                routeAdapter.UpdateItem(routeWithLocation);
            }
        }

        public async Task RequestLocationPermission()
        {
            string permission = Manifest.Permission.AccessFineLocation;
            if (Activity.CheckSelfPermission(permission) == Permission.Granted)
            {
                await StartListeningLocation();
                return;
            }

            if (ShouldShowRequestPermissionRationale(permission))
            {
                Snackbar.Make(feedRecyclerView, "Localisation désactivé", Snackbar.LengthLong)
                        .SetAction("Activer", v => RequestPermissions(PermissionsLocation, REQUEST_LOCATION_ID))
                        .Show();
                routeAdapter.IsLocationVisible = false;
                return;
            }

            RequestPermissions(PermissionsLocation, REQUEST_LOCATION_ID);
        }

        private async Task StartListeningLocation()
        {
            ConnectionResult connectionError = ((LocationService)Dependency.LocationService).ConnectionError;
            if (connectionError != null)
            {
                routeAdapter.IsLocationVisible = false;
                IsListeningToLocationChange = false;

                GoogleApiAvailability googleAPI = GoogleApiAvailability.Instance;
                if (googleAPI.IsUserResolvableError(connectionError.ErrorCode))
                {
                    googleAPI.GetErrorDialog(Activity, connectionError.ErrorCode, REQUEST_GOOGLE_PLAY_SERVICES).Show();
                }

                return;
            }

            routeAdapter.IsLocationVisible = true;

            if (Dependency.LocationService.LastKnownLocation != null)
            {
                await UpdateRoutesLocation(Dependency.LocationService.LastKnownLocation);
            }

            IsListeningToLocationChange = true;
            Dependency.LocationService.AddOnLocationChangedListener(this);
        }
    }
}