using System;
using Android.Support.V4.App;
using System.Collections.Generic;
using RosebudAppAndroid.Fragments;
using RosebudAppCore.Model;
using Android.Views;
using System.Linq;
using RosebudAppCore.Model.Enum;
using RosebudAppCore.Utils;

namespace RosebudAppAndroid.Adapters
{
    public class FavoriteRouteDirectionPagerAdapter : FragmentPagerAdapter
    {
        protected Dictionary<int, FavoriteRouteDirectionFragment> Fragments = new Dictionary<int, FavoriteRouteDirectionFragment>();
        protected FavoriteRoute FavoriteRoute;

        public event EventHandler<StopClickedEventArgs> ItemClicked;

        public FavoriteRouteDirectionPagerAdapter(FragmentManager fragmentManager, FavoriteRoute favoriteRoute) : base(fragmentManager)
        {
            FavoriteRoute = favoriteRoute;
        }

        public override int Count {
            get { return FavoriteRoute.Directions.Count; }
        }

        public override Fragment GetItem(int position)
        {
            FavoriteRouteDirectionFragment fragment = new FavoriteRouteDirectionFragment();

            return fragment;
        }

        /*public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(FavoriteRoute.Directions[position].Stop.stop_name);
        }*/

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            object objet = base.InstantiateItem(container, position);
            FavoriteRouteDirectionFragment fragment = (FavoriteRouteDirectionFragment)base.InstantiateItem(container, position);

            FavoriteRouteDirection direction = FavoriteRoute.Directions[position];
            fragment.ChangeItem(FavoriteRoute.Route, direction);

            /*fragment.RouteInfo = RouteInfo;
            fragment.Stops = itemStops[position];
            fragment.ItemClicked += OnItemClicked;
            fragments.Add(position, fragment);*/

            return fragment;
        }

        public override int GetItemPosition(Java.Lang.Object objectValue)
        {
            FavoriteRouteDirectionFragment fragment = (FavoriteRouteDirectionFragment)objectValue;
            int position = Fragments.FirstOrDefault(f => f.Value == fragment).Key;

            if (FavoriteRoute.Directions.Count > position)
            {
                fragment.ChangeItem(FavoriteRoute.Route, FavoriteRoute.Directions[position]);
            }

            return base.GetItemPosition(objectValue);
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object objectValue)
        {
            Fragments.Remove(position);
            base.DestroyItem(container, position, objectValue);
        }

        void OnItemClicked(object sender, StopClickedEventArgs e)
        {
            ItemClicked?.Invoke(sender, e);
        }

        public void ReplaceItem(FavoriteRoute favoriteRoute)
        {
            FavoriteRoute = favoriteRoute;
            NotifyDataSetChanged();
        }
    }
}