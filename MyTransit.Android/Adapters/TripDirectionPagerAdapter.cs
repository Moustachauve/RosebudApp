using System;
using Android.Support.V4.App;
using Android.Hardware.Camera2;
using MyTransit.Core;
using System.Collections.Generic;
using MyTransit.Android.Fragments;
using MyTransit.Core.Model;
using Android.Views.Animations;
using Android.Runtime;

namespace MyTransit.Android
{
	public class TripDirectionPagerAdapter : FragmentStatePagerAdapter
	{
		protected List<TripListFragment> fragments = new List<TripListFragment>();
		private RouteDetails routeDetails;

		public event EventHandler<TripClickedEventArgs> ItemClicked;

		public TripDirectionPagerAdapter(FragmentManager fragmentManager, RouteDetails routeDetails) : base(fragmentManager)
		{
			this.routeDetails = routeDetails;
			if (routeDetails.HasMultipleDirection())
			{
				CreateFragment(routeDetails.GetTripsForDirection(TripDirection.MainDirection));
				CreateFragment(routeDetails.GetTripsForDirection(TripDirection.OppositeDirection));
			}
			else 
			{
				CreateFragment(routeDetails.GetTripsForDirection(TripDirection.AnyDirection));
			}
		}

		public override int Count { get { return fragments.Count; } }

		public override Fragment GetItem(int position)
		{
			return fragments[position];
		}

		public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
		{
			return new Java.Lang.String(routeDetails.GetDirectionName((TripDirection)position));
		}

		private void CreateFragment(List<Trip> trips) 
		{
			TripListFragment fragment = new TripListFragment();
			fragment.SetTrips(trips);
			fragment.ItemClicked += OnItemClicked;
			fragments.Add(fragment);
		}

		private void OnItemClicked(object sender, TripClickedEventArgs e) {
			if (ItemClicked != null)
				ItemClicked(sender, e);
		}
	}
}

