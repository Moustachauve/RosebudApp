using System;
using RosebudAppCore.Model;
using RosebudAppCore.DataAccessor;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Android.Drm;

namespace RosebudAppCore.Utils
{
    public static class RouteLocationHelper
    {
        public static async Task<StopLocation> GetClosestStop(Route route, double lat, double lon)
        {
            StopLocation stopLocation = new StopLocation();
            List<Stop> stops = await RouteAccessor.GetRouteStops(route.feed_id, route.route_id, Dependency.PreferenceManager.SelectedDatetime, false);

            if (stops != null && stops.Count > 0)
            {
                Stop closestStop = stops[0];
                int closestStopInMeter = int.MaxValue;

                foreach (var stop in stops)
                {
                    int currentStopDistance = Dependency.LocationService.CalculateDistance(lat, lon, stop.stop_lat, stop.stop_lon);

                    if (currentStopDistance < closestStopInMeter)
                    {
                        closestStop = stop;
                        closestStopInMeter = currentStopDistance;
                    }
                }

                stopLocation.Stop = closestStop;
                stopLocation.DistanceInMeter = closestStopInMeter;
            }

            return stopLocation;
        }

        public static async Task GetNextTimeForStop(Route route, StopLocation stopLocation)
        {
            if (stopLocation.Stop == null)
                return;

            List<StopTime> stopTimes = await StopAccessor.GetStopTimes(route.feed_id, route.route_id, stopLocation.Stop.stop_id, DateTime.Now, false);

            DateTime now = DateTime.Now;
            DateTime closestTime = DateTime.MaxValue;

            foreach (var stopTime in stopTimes)
            {
                DateTime currentItemTime = TimeFormatter.StringToDateTime(stopTime.departure_time);
                if (currentItemTime < now)
                    continue;

                if(currentItemTime < closestTime)
                {
                    closestTime = currentItemTime;
                }
            }

            if(closestTime != DateTime.MaxValue)
            {
                stopLocation.NextDepartureTime = closestTime;
            }
        }

        public static string FormatDistance(int meter)
        {
            if (meter < 1000)
            {
                return meter + "m";
            }

            if (meter < 100000)
            {
                string distance = (meter / 1000.0).ToString("F");
                return distance + "km";
            }

            else
                return "+100km";
        }
    }
}
