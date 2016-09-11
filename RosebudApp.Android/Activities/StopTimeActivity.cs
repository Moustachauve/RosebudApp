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
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using ToolbarCompat = Android.Support.V7.Widget.Toolbar;
using RosebudAppAndroid.Fragments;
using Android.Support.V4.View;
using Newtonsoft.Json;
using RosebudAppCore.Utils;
using RosebudAppCore.Model;
using Android.Views.Animations;
using System.Threading.Tasks;
using RosebudAppCore;
using RosebudAppCore.DataAccessor;
using RosebudAppAndroid.Adapters;
using Android.Support.V7.Widget;
using Com.Tonicartos.Superslim;
using Android.Support.V4.Widget;

namespace RosebudAppAndroid.Activities
{
    [Activity(Label = "StopTimesActivity")]
    public class StopTimeActivity : AppCompatActivity
    {
        const string STATE_RECYCLER_VIEW = "state-recycler-view";

        Route routeInfo;
        Stop stopInfo;

        RecyclerView stopTimeRecyclerView;
        StopTimeAdapter stopTimeAdapter;
        SwipeRefreshLayout stopTimePullToRefresh;
        SwipeRefreshLayout stopTimePullToRefreshEmpty;


        LinearLayout emptyView;
        LinearLayout emptyViewNoInternet;
        TextView emptyViewMainText;

        IParcelable recyclerViewLayoutState;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.stop_time);

            var toolbar = FindViewById<ToolbarCompat>(Resource.Id.my_awesome_toolbar);
            NetworkStatusFragment networkFragment = (NetworkStatusFragment)FragmentManager.FindFragmentById(Resource.Id.network_fragment);

            stopTimeRecyclerView = FindViewById<RecyclerView>(Resource.Id.stop_time_recyclerview);
            emptyView = FindViewById<LinearLayout>(Resource.Id.empty_view);
            emptyViewNoInternet = FindViewById<LinearLayout>(Resource.Id.empty_view_no_internet);
            emptyViewMainText = FindViewById<TextView>(Resource.Id.empty_view_main_text);

            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            routeInfo = JsonConvert.DeserializeObject<Route>(Intent.GetStringExtra("routeInfos"));
            stopInfo = JsonConvert.DeserializeObject<Stop>(Intent.GetStringExtra("stopInfos"));
            Title = routeInfo.route_short_name + " - " + stopInfo.stop_name;

            stopTimePullToRefresh = FindViewById<SwipeRefreshLayout>(Resource.Id.stop_time_pull_to_refresh);
            stopTimePullToRefreshEmpty = FindViewById<SwipeRefreshLayout>(Resource.Id.stop_time_pull_to_refresh_empty);
            stopTimePullToRefresh.SetColorSchemeResources(Resource.Color.refresh_progress_1, Resource.Color.refresh_progress_2, Resource.Color.refresh_progress_3);
            stopTimePullToRefreshEmpty.SetColorSchemeResources(Resource.Color.refresh_progress_1, Resource.Color.refresh_progress_2, Resource.Color.refresh_progress_3);


            stopTimePullToRefresh.Refresh += PullToRefresh_Refresh;
            stopTimePullToRefreshEmpty.Refresh += PullToRefresh_Refresh;

            toolbar.NavigationClick += delegate
            {
                OnBackPressed();
            };

            networkFragment.RetryLastRequest += async (object sender, EventArgs args) =>
            {
                await LoadStopTimes();
            };

            toolbar.Post(async () =>
            {
                await LoadStopTimes();
                int scrollPosition = stopTimeAdapter.GetNearestTimeSectionPosition(DateTime.Now);
                stopTimeRecyclerView.SmoothScrollToPosition(scrollPosition);
            });
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutParcelable(STATE_RECYCLER_VIEW, stopTimeRecyclerView.GetLayoutManager().OnSaveInstanceState());
            base.OnSaveInstanceState(outState);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            recyclerViewLayoutState = (IParcelable)savedInstanceState.GetParcelable(STATE_RECYCLER_VIEW);
            base.OnRestoreInstanceState(savedInstanceState);
        }


        private async void PullToRefresh_Refresh(object sender, EventArgs e)
        {
            await LoadStopTimes(true);
        }

        async Task LoadStopTimes(bool overrideCache = false)
        {
            stopTimePullToRefresh.Refreshing = true;
            stopTimePullToRefreshEmpty.Refreshing = true;

            List<StopTime> stopTimes = await StopAccessor.GetStopTimes(routeInfo.feed_id, routeInfo.route_id, stopInfo.stop_id, Dependency.PreferenceManager.SelectedDatetime, overrideCache);

            UpdateEmptyMessage(stopTimes);

            if (stopTimeAdapter == null)
            {
                stopTimeAdapter = new StopTimeAdapter(this, stopTimes);
                stopTimeAdapter.ItemClick += OnItemClicked;
                stopTimeRecyclerView.SetAdapter(stopTimeAdapter);
                stopTimeRecyclerView.SetLayoutManager(new LayoutManager(this));

                if (recyclerViewLayoutState != null)
                {
                    stopTimeRecyclerView.GetLayoutManager().OnRestoreInstanceState(recyclerViewLayoutState);
                }
            }
            else
            {
                stopTimeAdapter.ReplaceItems(stopTimes);
            }

            stopTimePullToRefresh.Refreshing = false;
            stopTimePullToRefreshEmpty.Refreshing = false;
        }

        void UpdateEmptyMessage(List<StopTime> stopTimes)
        {
            if (stopTimes == null || stopTimes.Count == 0)
            {
                stopTimePullToRefresh.Visibility = ViewStates.Gone;
                stopTimePullToRefreshEmpty.Visibility = ViewStates.Visible;

                if (Dependency.NetworkStatusMonitor.CanConnect)
                {
                    emptyView.Visibility = ViewStates.Visible;
                    emptyViewNoInternet.Visibility = ViewStates.Gone;
                }
                else
                {
                    emptyView.Visibility = ViewStates.Gone;
                    emptyViewNoInternet.Visibility = ViewStates.Visible;
                }
            }
            else
            {
                stopTimePullToRefresh.Visibility = ViewStates.Visible;
                stopTimePullToRefreshEmpty.Visibility = ViewStates.Gone;
            }
        }

        void OnItemClicked(object sender, int position)
        {
            StopTime stopTime = ((StopTimeAdapter.ObjectItem)stopTimeAdapter[position]).Item;

            Trip tripInfo = new Trip();
            tripInfo.trip_id = stopTime.trip_id;
            tripInfo.trip_headsign = stopInfo.trip_headsign;

            Intent detailsIntent = new Intent(this, typeof(TripDetailsActivity));
            detailsIntent.PutExtra("routeInfos", JsonConvert.SerializeObject(routeInfo));
            detailsIntent.PutExtra("tripInfos", JsonConvert.SerializeObject(tripInfo));
            detailsIntent.PutExtra("stopId", stopInfo.stop_id);

            StartActivity(detailsIntent);
        }

    }
}