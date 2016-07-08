
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
using MyTransit.Android.Adapters;
using MyTransit.Core.Model;
using Newtonsoft.Json;
using FragmentSupport = Android.Support.V4.App.Fragment;
using System.Threading.Tasks;
using System.Threading;

namespace MyTransit.Android.Fragments
{
	public class TripListFragment : FragmentSupport
	{
		private TripAdapter tripAdapter;
		private List<Trip> trips;
		private Context context;

		private bool isViewLoaded = false;

		private RecyclerView tripRecyclerView;

		public event EventHandler<TripClickedEventArgs> ItemClicked;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate(Resource.Layout.trip_list, container, false);

			tripRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.trip_recyclerview);
			tripRecyclerView.NestedScrollingEnabled = false;

			isViewLoaded = true;

			return view;
		}

		public override void OnAttach(Context context)
		{
			this.context = context;
			if (trips != null)
				SetTrips(trips);

			base.OnAttach(context);
		}

		private void OnItemClick(object sender, int position)
		{
			if (ItemClicked == null)
				return;

			Trip clickedTrip = tripAdapter[position];
			ItemClicked(this, new TripClickedEventArgs(clickedTrip));
		}

		public async void SetTrips(List<Trip> trips)
		{
			this.trips = trips;

			await WaitForViewAndContext();

			if (tripAdapter == null)
			{
				tripAdapter = new TripAdapter(Activity, trips);
				tripAdapter.ItemClick += OnItemClick;
				tripRecyclerView.SetAdapter(tripAdapter);
				tripRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
			}
			else {
				tripAdapter.ReplaceItems(trips);
			}

			tripRecyclerView.Post(() =>
			{
				//TODO
				//tripRecyclerView.SmoothScrollToPositionFromTop(tripAdapter.GetPositionOfNextTrip(), 50);
			});
		}

		private async Task WaitForViewAndContext()
		{
			while (isViewLoaded == false || context == null)
			{
				await Task.Delay(200);
			}
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

