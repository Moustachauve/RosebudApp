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
        private bool isLocationVisible = true;
        public bool IsLocationVisible
        {
            get { return isLocationVisible; }
            set
            {
                if (value == isLocationVisible)
                    return;

                isLocationVisible = value;
                NotifyDataSetChanged();
            }
        }

        public RouteWithStopLocationAdapter(Context context, List<RouteWithStopLocation> routesWithStopLocation) : base(context, routesWithStopLocation)
        {
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = Inflater.Inflate(Resource.Layout.route_stop_location_listitem, parent, false);

            RouteWithStopLocationViewHolder viewHolder = new RouteWithStopLocationViewHolder(view, OnClick, this);

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
            TimeComparator timeComparator = new TimeComparator();

            return AllItems.OrderByDescending(r => r.StopLocation != null && r.StopLocation.NextDepartureTime != DateTime.MinValue)
                           .ThenBy(r => r.StopLocation, distanceComparator)
                           .ThenBy(r => r.StopLocation, timeComparator)
                           .ThenBy(r => r.Route.route_short_name, naturalComparator)
                           .ToList();
        }

        public void UpdateItem(RouteWithStopLocation route)
        {
            for (int i = 0; i < AllItems.Count; i++)
            {
                if (AllItems[i].Route.route_id != route.Route.route_id)
                    continue;

                AllItems[i].Route = route.Route;
                AllItems[i].StopLocation = route.StopLocation;

                NotifyItemChanged(i);
                AllItems = ApplySort();
                AnimateTo(AllItems);

                break;
            }
        }

        public class RouteWithStopLocationViewHolder : BaseViewHolder
        {
            TextView lblRouteShortName;
            TextView lblRouteLongName;
            TextView lblRouteTimeNextBus;
            TextView lblRouteClosestStop;
            TextView lblRouteClosestStopDistance;
            TextView lblRouteNextDeparture;
            TextView lblRouteNextDepartureTimeUnit;
            ProgressBar progressTimeRoute;

            RouteWithStopLocation currentItem;

            RouteWithStopLocationAdapter Adapter;

            public RouteWithStopLocationViewHolder(View itemView, Action<int> listener, RouteWithStopLocationAdapter adapter) : base(itemView, listener)
            {
                lblRouteShortName = view.FindViewById<TextView>(Resource.Id.lbl_route_short_name);
                lblRouteLongName = view.FindViewById<TextView>(Resource.Id.lbl_route_long_name);
                progressTimeRoute = view.FindViewById<ProgressBar>(Resource.Id.progress_time_route);
                lblRouteTimeNextBus = view.FindViewById<TextView>(Resource.Id.lbl_route_time_next_bus);
                lblRouteClosestStop = view.FindViewById<TextView>(Resource.Id.lbl_route_closest_stop);
                lblRouteClosestStopDistance = view.FindViewById<TextView>(Resource.Id.lbl_route_closest_stop_distance);
                lblRouteNextDeparture = view.FindViewById<TextView>(Resource.Id.lbl_route_next_departure);
                lblRouteNextDepartureTimeUnit = view.FindViewById<TextView>(Resource.Id.lbl_route_next_departure_time_unit);

                Adapter = adapter;
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
                    routeColor = new Color(ContextCompat.GetColor(Adapter.Context, Resource.Color.default_item_color));
                }

                lblRouteShortName.SetBackgroundColor(routeColor);
                lblRouteShortName.SetTextColor(ColorHelper.ContrastColor(routeColor));

            }

            private void SetRouteLocationInfo()
            {
                if (!Adapter.IsLocationVisible)
                {
                    HideLocation();
                    return;
                }

                lblRouteNextDeparture.Visibility = ViewStates.Visible;

                if (currentItem.StopLocation == null)
                {
                    if (Dependency.NetworkStatusMonitor.State == NetworkState.Disconnected)
                    {
                        HideLocation();
                    }
                    else
                    {
                        progressTimeRoute.Visibility = ViewStates.Visible;
                        lblRouteClosestStop.Visibility = ViewStates.Gone;
                        lblRouteClosestStopDistance.Visibility = ViewStates.Gone;
                        lblRouteTimeNextBus.Visibility = ViewStates.Gone;
                        lblRouteNextDepartureTimeUnit.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    if (currentItem.StopLocation.Stop == null)
                    {
                        lblRouteClosestStop.Text = Adapter.Context.Resources.GetString(Resource.String.route_next_departure_no_service);
                        lblRouteTimeNextBus.Text = "--";
                        lblRouteClosestStopDistance.Visibility = ViewStates.Gone;
                        lblRouteNextDepartureTimeUnit.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        lblRouteClosestStop.Text = currentItem.StopLocation.Stop.stop_name;
                        lblRouteClosestStopDistance.Text = RouteLocationHelper.FormatDistance(currentItem.StopLocation.DistanceInMeter);

                        SetRemainingTime();

                        lblRouteClosestStopDistance.Visibility = ViewStates.Visible;
                    }

                    progressTimeRoute.Visibility = ViewStates.Gone;
                    lblRouteClosestStop.Visibility = ViewStates.Visible;
                    lblRouteTimeNextBus.Visibility = ViewStates.Visible;
                    lblRouteNextDepartureTimeUnit.Visibility = ViewStates.Visible;
                }
            }

            void HideLocation()
            {
                progressTimeRoute.Visibility = ViewStates.Gone;
                lblRouteNextDeparture.Visibility = ViewStates.Gone;
                lblRouteClosestStop.Visibility = ViewStates.Gone;
                lblRouteClosestStopDistance.Visibility = ViewStates.Gone;
                lblRouteTimeNextBus.Visibility = ViewStates.Gone;
                lblRouteNextDepartureTimeUnit.Visibility = ViewStates.Gone;
            }

            void SetRemainingTime()
            {
                if (currentItem.StopLocation.NextDepartureTime == DateTime.MinValue)
                {
                    lblRouteTimeNextBus.Text = "--";
                    lblRouteNextDepartureTimeUnit.Visibility = ViewStates.Gone;
                    return;
                }

                TimeSpan difference = currentItem.StopLocation.NextDepartureTime - DateTime.Now;

                int timeDisplayed = 0;

                if (difference.TotalMinutes < 120)
                {
                    timeDisplayed = (int)Math.Round(difference.TotalMinutes);

                    if (timeDisplayed > 1)
                        lblRouteNextDepartureTimeUnit.Text = Adapter.Context.Resources.GetString(Resource.String.minutes);
                    else
                        lblRouteNextDepartureTimeUnit.Text = Adapter.Context.Resources.GetString(Resource.String.minute);
                }
                else
                {
                    timeDisplayed = (int)Math.Round(difference.TotalHours);

                    if (timeDisplayed > 1)
                        lblRouteNextDepartureTimeUnit.Text = Adapter.Context.Resources.GetString(Resource.String.hours);
                    else
                        lblRouteNextDepartureTimeUnit.Text = Adapter.Context.Resources.GetString(Resource.String.hour);
                }

                lblRouteTimeNextBus.Text = timeDisplayed.ToString();
            }
        }
    }
}
