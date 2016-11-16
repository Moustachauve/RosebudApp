using System;
using RosebudAppCore.Model;
using RosebudAppCore.DataAccessor;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Android.Drm;
using RosebudAppCore.Model.Enum;

namespace RosebudAppCore.Utils
{
    public static class RouteLocationHelper
    {
        public static async Task<List<FavoriteRouteDirection>> GetClosestStops(Route route, double lat, double lon)
        {
            List<FavoriteRouteDirection> favoriteRouteDirections = new List<FavoriteRouteDirection>();
            List<Stop> stops = await RouteAccessor.GetRouteStops(route.feed_id, route.route_id, Dependency.PreferenceManager.SelectedDatetime, false);

            if (stops == null)
                return favoriteRouteDirections;

            if (TripDirectionHelper.HasMultipleDirection(stops))
            {
                favoriteRouteDirections.Add(GetClosestStopByDirection(stops, TripDirection.MainDirection, lat, lon));
                favoriteRouteDirections.Add(GetClosestStopByDirection(stops, TripDirection.OppositeDirection, lat, lon));
            }
            else
            {
                favoriteRouteDirections.Add(GetClosestStopByDirection(stops, TripDirection.AnyDirection, lat, lon));
            }


            return favoriteRouteDirections;
        }

        private static FavoriteRouteDirection GetClosestStopByDirection(List<Stop> stops, TripDirection direction, double lat, double lon)
        {
            FavoriteRouteDirection stopLocation = new FavoriteRouteDirection();

            if (stops != null && stops.Count > 0)
            {
                List<Stop> directionStops = TripDirectionHelper.GetStopsForDirection(stops, direction);

                Stop closestStop = directionStops[0];
                int closestStopInMeter = int.MaxValue;

                foreach (var stop in directionStops)
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


        public static async Task GetNextTimeForFavoriteRoute(FavoriteRoute favoriteRoute)
        {
            if (favoriteRoute.Directions == null || favoriteRoute.Directions.Count == 0)
                return;

            foreach (var direction in favoriteRoute.Directions)
            {
                if (direction.Stop == null)
                    continue;

                List<StopTime> stopTimes = await StopAccessor.GetStopTimes(favoriteRoute.Route.feed_id, favoriteRoute.Route.route_id, direction.Stop.stop_id, direction.Stop.direction_id, DateTime.Now, false);

                if (stopTimes == null)
                    continue;

                DateTime now = DateTime.Now;
                DateTime closestTime = DateTime.MaxValue;

                foreach (var stopTime in stopTimes)
                {
                    DateTime currentItemTime = TimeFormatter.StringToDateTime(stopTime.departure_time);
                    if (currentItemTime < now)
                        continue;

                    if (currentItemTime < closestTime)
                    {
                        closestTime = currentItemTime;
                    }
                }

                if (closestTime != DateTime.MaxValue)
                {
                    direction.NextDepartureTime = closestTime;
                }
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
