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

        public static async Task GetNextTimeForStop()
        {

        }
    }
}
