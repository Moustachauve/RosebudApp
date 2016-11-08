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
using RosebudAppCore.Model.Enum;
using RosebudAppCore.Utils;

namespace RosebudAppAndroid
{
    public class StopDirectionPagerAdapter : FragmentPagerAdapter
    {
        protected Dictionary<int, StopListFragment> fragments = new Dictionary<int, StopListFragment>();
        protected List<List<Stop>> itemStops = new List<List<Stop>>();
        List<Stop> allStops = new List<Stop>();
        Route RouteInfo;

        public event EventHandler<StopClickedEventArgs> ItemClicked;

        public StopDirectionPagerAdapter(FragmentManager fragmentManager, Route routeInfo) : base(fragmentManager)
        {
            this.RouteInfo = routeInfo;
        }

        public override int Count { get { return itemStops.Count; } }

        public override Fragment GetItem(int position)
        {
            StopListFragment fragment = new StopListFragment();

            return fragment;
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            if (Count <= 1)
            {
                return new Java.Lang.String(TripDirectionHelper.GetDirectionName(allStops, TripDirection.AnyDirection));
            }

            return new Java.Lang.String(TripDirectionHelper.GetDirectionName(allStops, (TripDirection)position));
        }

        public void UpdateStops(List<Stop> stops)
        {
            itemStops.Clear();

            if (stops == null)
            {
                allStops = new List<Stop>();
            }
            else
            {
                allStops = new List<Stop>(stops);

                if (TripDirectionHelper.HasMultipleDirection(stops))
                {
                    itemStops.Add(TripDirectionHelper.GetStopsForDirection(allStops, TripDirection.MainDirection));
                    itemStops.Add(TripDirectionHelper.GetStopsForDirection(allStops, TripDirection.OppositeDirection));
                }
                else
                {
                    itemStops.Add(TripDirectionHelper.GetStopsForDirection(allStops, TripDirection.AnyDirection));
                }
            }

            NotifyDataSetChanged();
        }

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            StopListFragment fragment = (StopListFragment)base.InstantiateItem(container, position);

            fragment.RouteInfo = RouteInfo;
            fragment.Stops = itemStops[position];
            fragment.ItemClicked += OnItemClicked;
            fragments.Add(position, fragment);

            return fragment;
        }

        public override int GetItemPosition(Java.Lang.Object objectValue)
        {
            StopListFragment fragment = (StopListFragment)objectValue;
            int position = fragments.FirstOrDefault(f => f.Value == fragment).Key;

            if (itemStops.Count < position)
            {
                fragment.Stops = itemStops[position];
            }

            return base.GetItemPosition(objectValue);
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object objectValue)
        {
            fragments.Remove(position);
            base.DestroyItem(container, position, objectValue);
        }

        void OnItemClicked(object sender, StopClickedEventArgs e)
        {
            ItemClicked?.Invoke(sender, e);
        }
    }
}

