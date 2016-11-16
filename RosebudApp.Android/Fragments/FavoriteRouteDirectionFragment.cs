using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FragmentSupport = Android.Support.V4.App.Fragment;
using RosebudAppCore.Model;
using Android.Graphics;
using RosebudAppCore;
using Android.Support.V4.Content;
using RosebudAppCore.Utils;

namespace RosebudAppAndroid.Fragments
{
    public class FavoriteRouteDirectionFragment : FragmentSupport
    {
        public FavoriteRouteDirection Direction { get; set; }
        public Route Route { get; set; }

        TextView lblRouteLongName;
        TextView lblRouteTimeNextBus;
        TextView lblRouteClosestStop;
        TextView lblRouteClosestStopDistance;
        TextView lblRouteNextDepartureTimeUnit;
        ProgressBar progressTimeRoute;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.route_stop_location_listitem_direction, container, false);

            progressTimeRoute = view.FindViewById<ProgressBar>(Resource.Id.progress_time_route);
            lblRouteLongName = view.FindViewById<TextView>(Resource.Id.lbl_route_long_name);
            lblRouteTimeNextBus = view.FindViewById<TextView>(Resource.Id.lbl_route_time_next_bus);
            lblRouteClosestStop = view.FindViewById<TextView>(Resource.Id.lbl_route_closest_stop);
            lblRouteClosestStopDistance = view.FindViewById<TextView>(Resource.Id.lbl_route_closest_stop_distance);
            lblRouteNextDepartureTimeUnit = view.FindViewById<TextView>(Resource.Id.lbl_route_next_departure_time_unit);

            return view;
        }

        public override void OnStart()
        {
            base.OnStart();

            if (Route != null)
                ChangeItem(Route, Direction);
        }

        public void ChangeItem(Route route, FavoriteRouteDirection direction)
        {
            Route = route;
            Direction = direction;

            if (progressTimeRoute != null)
            {
                SetRouteLongName();
                SetRouteLocationInfo();
            }
        }

        private void SetRouteLongName()
        {
            if (!string.IsNullOrWhiteSpace(Route.route_long_name))
            {
                lblRouteLongName.Text = Route.route_long_name;
            }
            else
            {
                lblRouteLongName.Text = Route.route_desc;
            }
        }


        private void SetRouteLocationInfo()
        {
            /*if (!Adapter.IsLocationVisible)
            {
                HideLocation();
                return;
            }*/

            if (Direction == null)
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
                if (Direction.Stop == null)
                {
                    lblRouteClosestStop.Text = Context.Resources.GetString(Resource.String.route_next_departure_no_service);
                    lblRouteTimeNextBus.Text = "--";
                    lblRouteClosestStopDistance.Visibility = ViewStates.Gone;
                    lblRouteNextDepartureTimeUnit.Visibility = ViewStates.Gone;
                }
                else
                {
                    lblRouteClosestStop.Text = Direction.Stop.stop_name;
                    lblRouteClosestStopDistance.Text = RouteLocationHelper.FormatDistance(Direction.DistanceInMeter);

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
            lblRouteClosestStop.Visibility = ViewStates.Gone;
            lblRouteClosestStopDistance.Visibility = ViewStates.Gone;
            lblRouteTimeNextBus.Visibility = ViewStates.Gone;
            lblRouteNextDepartureTimeUnit.Visibility = ViewStates.Gone;
        }

        void SetRemainingTime()
        {
            if (Direction.NextDepartureTime == DateTime.MinValue)
            {
                lblRouteTimeNextBus.Text = "--";
                lblRouteNextDepartureTimeUnit.Visibility = ViewStates.Gone;
                return;
            }

            TimeSpan difference = Direction.NextDepartureTime - DateTime.Now;

            int timeDisplayed = 0;

            if (difference.TotalMinutes < 120)
            {
                timeDisplayed = (int)Math.Round(difference.TotalMinutes);

                if (timeDisplayed > 1)
                    lblRouteNextDepartureTimeUnit.Text = Context.Resources.GetString(Resource.String.minutes);
                else
                    lblRouteNextDepartureTimeUnit.Text = Context.Resources.GetString(Resource.String.minute);
            }
            else
            {
                timeDisplayed = (int)Math.Round(difference.TotalHours);

                if (timeDisplayed > 1)
                    lblRouteNextDepartureTimeUnit.Text = Context.Resources.GetString(Resource.String.hours);
                else
                    lblRouteNextDepartureTimeUnit.Text = Context.Resources.GetString(Resource.String.hour);
            }

            lblRouteTimeNextBus.Text = timeDisplayed.ToString();
        }
    }
}