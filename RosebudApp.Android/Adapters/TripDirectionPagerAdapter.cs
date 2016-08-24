using System;
using Android.Support.V4.App;
using Android.Hardware.Camera2;
using RosebudAppCore;
using System.Collections.Generic;
using RosebudAppAndroid.Fragments;
using RosebudAppCore.Model;
using Android.Views.Animations;
using Android.Runtime;
using Android.Views;
using Java.Lang;
using System.Linq;

namespace RosebudAppAndroid
{
    public class TripDirectionPagerAdapter : FragmentPagerAdapter
    {
        protected Dictionary<int, TripListFragment> fragments = new Dictionary<int, TripListFragment>();
        protected List<List<Trip>> itemTrips = new List<List<Trip>>();
        RouteDetails routeDetails;

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

            fragment.Trips = itemTrips[position];
            fragment.ItemClicked += OnItemClicked;
            fragments.Add(position, fragment);

            return fragment;
        }

        public override int GetItemPosition(Java.Lang.Object objectValue)
        {
            TripListFragment fragment = (TripListFragment)objectValue;
            int position = fragments.FirstOrDefault(f => f.Value == fragment).Key;

            if (itemTrips.Count < position)
            {
                fragment.Trips = itemTrips[position];
            }

            return base.GetItemPosition(objectValue);
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object objectValue)
        {
            fragments.Remove(position);
            base.DestroyItem(container, position, objectValue);
        }

        void OnItemClicked(object sender, TripClickedEventArgs e)
        {
            ItemClicked?.Invoke(sender, e);
        }
    }
}

