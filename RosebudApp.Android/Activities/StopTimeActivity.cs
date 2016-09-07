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

namespace RosebudAppAndroid.Activities
{
    [Activity(Label = "StopTimesActivity")]
    public class StopTimeActivity : AppCompatActivity
    {
        Route routeInfo;
        Stop stopInfo;

        RecyclerView stopTimeRecyclerView;
        StopTimeAdapter stopTimeAdapter;

        LinearLayout emptyView;
        LinearLayout emptyViewNoInternet;
        TextView emptyViewMainText;

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
            });
            
        }

        async Task LoadStopTimes(bool overrideCache = false)
        {
            List<StopTime> stopTimes = await StopAccessor.GetStopTimes(routeInfo.feed_id, routeInfo.route_id, stopInfo.stop_id, Dependency.PreferenceManager.SelectedDatetime, overrideCache);

            UpdateEmptyMessage(stopTimes);

            if (stopTimeAdapter == null)
            {
                stopTimeAdapter = new StopTimeAdapter(this, stopTimes);
                stopTimeAdapter.ItemClick += OnItemClicked;
                stopTimeRecyclerView.SetAdapter(stopTimeAdapter);
                stopTimeRecyclerView.SetLayoutManager(new LayoutManager(this));
            }
            else
            {
                stopTimeAdapter.ReplaceItems(stopTimes);
            }
        }

        void UpdateEmptyMessage(List<StopTime> stopTimes)
        {
            if (stopTimes == null || stopTimes.Count == 0)
            {
                stopTimeRecyclerView.Visibility = ViewStates.Gone;

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
                stopTimeRecyclerView.Visibility = ViewStates.Visible;
                emptyView.Visibility = ViewStates.Gone;
                emptyViewNoInternet.Visibility = ViewStates.Gone;
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