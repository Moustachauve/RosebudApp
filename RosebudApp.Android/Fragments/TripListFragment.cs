
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
    public class TripListFragment : FragmentSupport
    {
        const string BUNDLE_KEY_TRIPS = "bundle_trips_frag";

        TripAdapter tripAdapter;
        List<Trip> trips;

        bool isViewLoaded = false;

        RecyclerView tripRecyclerView;

        public event EventHandler<TripClickedEventArgs> ItemClicked;

        public List<Trip> Trips
        {
            get { return new List<Trip>(trips); }
            set
            {
                trips = new List<Trip>(value);
                UpdateTrips();
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.trip_list, container, false);

            if (savedInstanceState != null)
            {
                string jsonTrips = savedInstanceState.GetString(BUNDLE_KEY_TRIPS);
                if (!string.IsNullOrEmpty(jsonTrips))
                {
                    trips = JsonConvert.DeserializeObject<List<Trip>>(jsonTrips);
                }
            }

            tripRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.trip_recyclerview);
            tripRecyclerView.NestedScrollingEnabled = false;

            isViewLoaded = true;
            UpdateTrips();

            return view;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            string jsonTrips = JsonConvert.SerializeObject(trips);
            outState.PutString(BUNDLE_KEY_TRIPS, jsonTrips);
            base.OnSaveInstanceState(outState);
        }

        void OnItemClick(object sender, int position)
        {
            if (ItemClicked == null)
                return;

            Trip clickedTrip = tripAdapter[position];
            ItemClicked(this, new TripClickedEventArgs(clickedTrip));
        }

        void UpdateTrips()
        {
            if (trips == null)
                return;
            if (!isViewLoaded)
                return;

            if (tripAdapter == null)
            {
                tripAdapter = new TripAdapter(Activity, trips);
                tripAdapter.ItemClick += OnItemClick;
                tripRecyclerView.SetAdapter(tripAdapter);
                tripRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
            }
            else
            {
                tripAdapter.ReplaceItems(trips);
            }

            tripRecyclerView.Post(() =>
            {
                tripRecyclerView.ScrollToPosition(tripAdapter.GetPositionOfNextTrip());
            });
        }

    }
    public class TripClickedEventArgs : EventArgs
    {
        public Trip Trip { get; set; }

        public TripClickedEventArgs(Trip trip)
        {
            Trip = trip;
        }
    }

}

