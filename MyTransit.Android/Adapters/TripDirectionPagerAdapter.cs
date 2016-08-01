using System;
using Android.Support.V4.App;
using Android.Hardware.Camera2;
using MyTransitCore;
using System.Collections.Generic;
using MyTransitAndroid.Fragments;
using MyTransitCore.Model;
using Android.Views.Animations;
using Android.Runtime;
using Android.Views;
using Java.Lang;

namespace MyTransitAndroid
{
    public class TripDirectionPagerAdapter : FragmentPagerAdapter
    {
        protected List<TripListFragment> fragments = new List<TripListFragment>();
        protected List<List<Trip>> itemTrips = new List<List<Trip>>();
        private RouteDetails routeDetails;

        public event EventHandler<TripClickedEventArgs> ItemClicked;

        public TripDirectionPagerAdapter(FragmentManager fragmentManager) : base(fragmentManager)
        {
        }

        public override int Count { get { return itemTrips.Count; } }

        public override Fragment GetItem(int position)
        {
            TripListFragment fragment = new TripListFragment();

            return fragment;
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            if (Count <= 1)
            {
                return new Java.Lang.String(routeDetails.GetDirectionName(TripDirection.AnyDirection));
            }

            return new Java.Lang.String(routeDetails.GetDirectionName((TripDirection)position));
        }

        public void UpdateTrips(RouteDetails routeDetails)
        {
            this.routeDetails = routeDetails;
            itemTrips.Clear();

            if (routeDetails != null)
            {
                if (routeDetails.HasMultipleDirection())
                {
                    itemTrips.Add(new List<Trip>(routeDetails.GetTripsForDirection(TripDirection.MainDirection)));
                    itemTrips.Add(new List<Trip>(routeDetails.GetTripsForDirection(TripDirection.OppositeDirection)));
                }
                else
                {
                    itemTrips.Add(new List<Trip>(routeDetails.GetTripsForDirection(TripDirection.AnyDirection)));
                }
            }

            NotifyDataSetChanged();
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            TripListFragment fragment = (TripListFragment)base.InstantiateItem(container, position);

            fragment.Trips = itemTrips[0];
            fragment.ItemClicked += OnItemClicked;
            fragments.Insert(position, fragment);

            return fragment;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object objectValue)
        {
            fragments.RemoveAt(position);
            base.DestroyItem(container, position, objectValue);
        }

        private void OnItemClicked(object sender, TripClickedEventArgs e)
        {
            if (ItemClicked != null)
                ItemClicked(sender, e);
        }
    }
}

