using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using RosebudAppCore;
using Android.Graphics;
using System.Linq;
using RosebudAppCore.Model;
using RosebudAppCore.Utils;
using Android.Support.V4.Graphics;
using Android.Support.V7.Widget;
using RosebudAppCore.DataAccessor;
using RosebudAppCore.Comparators;
using RosebudAppAndroid.Adapters.Events;
using Android.Support.V4.Content;

namespace RosebudAppAndroid.Adapters
{
    public class RouteWithStopLocationAdapter : SearchableRecyclerAdapter<RouteWithStopLocation>
    {
        public event EventHandler<ItemCheckChangedEventArgs> ItemFavoriteClick;

        public RouteWithStopLocationAdapter(Context context, List<RouteWithStopLocation> routesWithStopLocation) : base(context, routesWithStopLocation)
        {
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = Inflater.Inflate(Resource.Layout.route_stop_location_listitem, parent, false);

            RouteWithStopLocationViewHolder viewHolder = new RouteWithStopLocationViewHolder(view, OnClick, Context);

            return viewHolder;
        }

        protected override List<RouteWithStopLocation> ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(Filter))
                return new List<RouteWithStopLocation>(AllItems);
            else
                return AllItems.Where(r => r.Route.route_short_name.ContainsInsensitive(Filter)
                                      || r.Route.route_long_name.ContainsInsensitive(Filter))
                                      .ToList();
        }

        protected override List<RouteWithStopLocation> ApplySort()
        {
            AlphanumComparator naturalComparator = new AlphanumComparator();
            DistanceComparator distanceComparator = new DistanceComparator();

            return AllItems.OrderBy(r => r.StopLocation, distanceComparator)
                           .ThenBy(r => r.Route.route_short_name, naturalComparator)
                           .ToList();
        }

        protected void OnFavoriteClick(object sender, ItemCheckChangedEventArgs e)
        {
            AnimateTo(ApplySort());
            ApplyFilter();
            ItemFavoriteClick?.Invoke(this, e);
        }

        public class RouteWithStopLocationViewHolder : BaseViewHolder
        {
            TextView lblRouteShortName;
            TextView lblRouteLongName;
            TextView lblRouteTimeNextBus;
            TextView lblRouteClosestStop;
            TextView lblRouteClosestStopDistance;
            ProgressBar progressTimeRoute;

            RouteWithStopLocation currentItem;

            Context Context;

            public RouteWithStopLocationViewHolder(View itemView, Action<int> listener, Context context) : base(itemView, listener)
            {
                lblRouteShortName = view.FindViewById<TextView>(Resource.Id.lbl_route_short_name);
                lblRouteLongName = view.FindViewById<TextView>(Resource.Id.lbl_route_long_name);
                progressTimeRoute = view.FindViewById<ProgressBar>(Resource.Id.progress_time_route);
                lblRouteTimeNextBus = view.FindViewById<TextView>(Resource.Id.lbl_route_time_next_bus);
                lblRouteClosestStop = view.FindViewById<TextView>(Resource.Id.lbl_route_closest_stop);
                lblRouteClosestStopDistance = view.FindViewById<TextView>(Resource.Id.lbl_route_closest_stop_distance);

                Context = context;
            }

            public override void BindData(RouteWithStopLocation item, int position)
            {
                currentItem = item;
                lblRouteShortName.Text = item.Route.route_short_name;

                SetRouteLongName();
                SetColor();
                SetRouteLocationInfo();
            }

            private void SetRouteLongName()
            {
                if (!string.IsNullOrWhiteSpace(currentItem.Route.route_long_name))
                {
                    lblRouteLongName.Text = currentItem.Route.route_long_name;
                }
                else
                {
                    lblRouteLongName.Text = currentItem.Route.route_desc;
                }
            }

            private void SetColor()
            {
                Color routeColor;
                if (!string.IsNullOrWhiteSpace(currentItem.Route.route_color))
                {
                    routeColor = Color.ParseColor(ColorHelper.FormatColor(currentItem.Route.route_color));
                }
                else 
                {
                    routeColor = new Color(ContextCompat.GetColor(Context, Resource.Color.default_item_color));
                }

                lblRouteShortName.SetBackgroundColor(routeColor);
                lblRouteShortName.SetTextColor(ColorHelper.ContrastColor(routeColor));

            }

            private void SetRouteLocationInfo()
            {
                if (currentItem.StopLocation == null)
                {
                    progressTimeRoute.Visibility = ViewStates.Visible;
                    lblRouteClosestStop.Visibility = ViewStates.Gone;
                    lblRouteClosestStopDistance.Visibility = ViewStates.Gone;
                    lblRouteTimeNextBus.Visibility = ViewStates.Gone;
                }
                else
                {
                    if (currentItem.StopLocation.Stop == null)
                    {
                        lblRouteClosestStop.Text = "No service today";
                        lblRouteTimeNextBus.Text = "--";
                        lblRouteClosestStopDistance.Visibility = ViewStates.Gone;
                    }
                    else 
                    {
                        lblRouteClosestStop.Text = currentItem.StopLocation.Stop.stop_name;
                        lblRouteClosestStopDistance.Text = currentItem.StopLocation.DistanceInMeter + "m";
                        lblRouteTimeNextBus.Text = "?? min";

                        lblRouteClosestStopDistance.Visibility = ViewStates.Visible;
                    }

                    progressTimeRoute.Visibility = ViewStates.Gone;
                    lblRouteClosestStop.Visibility = ViewStates.Visible;
                    lblRouteTimeNextBus.Visibility = ViewStates.Visible;
                }

            }

            async void ChkFavoriteCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                if (FavoriteRouteAccessor.IsRouteFavorite(currentItem.Route) == e.IsChecked)
                    return;

                await FavoriteRouteAccessor.SetFavoriteForRoute(e.IsChecked, currentItem.Route);
            }
        }
    }
}
