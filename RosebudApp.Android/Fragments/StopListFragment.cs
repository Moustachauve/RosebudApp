
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using RosebudAppAndroid.Adapters;
using RosebudAppCore.Model;
using Newtonsoft.Json;
using FragmentSupport = Android.Support.V4.App.Fragment;
using System.Threading.Tasks;
using System.Threading;

namespace RosebudAppAndroid.Fragments
{
    public class StopListFragment : FragmentSupport
    {
        const string BUNDLE_KEY_TRIPS = "bundle_trips_frag";

        StopAdapter stopAdapter;
        List<Stop> stops;

        bool isViewLoaded = false;

        RecyclerView stopRecyclerView;

        public event EventHandler<StopClickedEventArgs> ItemClicked;

        public List<Stop> Trips
        {
            get { return new List<Stop>(stops); }
            set
            {
                stops = new List<Stop>(value);
                UpdateTrips();
            }
        }

        public Route RouteInfo { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.trip_list, container, false);

            if (savedInstanceState != null)
            {
                string jsonTrips = savedInstanceState.GetString(BUNDLE_KEY_TRIPS);
                if (!string.IsNullOrEmpty(jsonTrips))
                {
                    stops = JsonConvert.DeserializeObject<List<Stop>>(jsonTrips);
                }
            }

            stopRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.trip_recyclerview);
            stopRecyclerView.NestedScrollingEnabled = false;

            isViewLoaded = true;
            UpdateTrips();

            return view;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            string jsonStops = JsonConvert.SerializeObject(stops);
            outState.PutString(BUNDLE_KEY_TRIPS, jsonStops);
            //outState.PutParcelable(STATE_RECYCLER_VIEW, stopRecyclerView.GetLayoutManager().OnSaveInstanceState());
            base.OnSaveInstanceState(outState);
        }

        void OnItemClick(object sender, int position)
        {
            if (ItemClicked == null)
                return;

            Stop clickedStop = stops[position];
            ItemClicked(this, new StopClickedEventArgs(clickedStop));
        }

        void UpdateTrips()
        {
            if (stops == null)
                return;
            if (!isViewLoaded)
                return;

            if (stopAdapter == null)
            {
                stopAdapter = new StopAdapter(Activity, stops, RouteInfo);
                stopAdapter.ItemClick += OnItemClick;
                stopRecyclerView.SetAdapter(stopAdapter);
                stopRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
            }
            else
            {
                stopAdapter.RouteInfo = RouteInfo;
                stopAdapter.ReplaceItems(stops);
            }

            stopRecyclerView.Post(() =>
            {
                //tripRecyclerView.ScrollToPosition(stopAdapter.GetPositionOfNextTrip());
            });
        }

    }
    public class StopClickedEventArgs : EventArgs
    {
        public Stop Stop { get; set; }

        public StopClickedEventArgs(Stop stop)
        {
            Stop = stop;
        }
    }

}

