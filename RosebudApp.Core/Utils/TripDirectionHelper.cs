using RosebudAppCore.Model;
using RosebudAppCore.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosebudAppCore.Utils
{
    public static class TripDirectionHelper
    {

        public static List<Trip> GetTripsForDirection(List<Trip> trips, TripDirection direction)
        {
            if (direction == TripDirection.AnyDirection)
                return trips;

            return trips.Where(t => t.direction_id == direction).ToList();
        }

        public static List<Stop> GetStopsForDirection(List<Stop> stops, TripDirection direction)
        {
            if (direction == TripDirection.AnyDirection)
                return stops;

            return stops.Where(t => t.direction_id == direction).ToList();
        }

        public static string GetDirectionName(List<Trip> trips, TripDirection direction)
        {
            if (trips == null || trips.Count == 0)
                return "";

            if (direction == TripDirection.AnyDirection)
            {
                Trip tripWithName = trips.FirstOrDefault(t => !string.IsNullOrWhiteSpace(t.trip_headsign));
                if (tripWithName == null)
                    return "";

                return tripWithName.trip_headsign;
            }

            return trips.FirstOrDefault(t => t.direction_id == direction).trip_headsign;
        }

        public static string GetDirectionName(List<Stop> stops, TripDirection direction)
        {
            if (stops == null || stops.Count == 0)
                return "";

            if (direction == TripDirection.AnyDirection)
            {
                Stop stopWithName = stops.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s.trip_headsign));
                if (stopWithName == null)
                    return "";

                return stopWithName.trip_headsign;
            }

            return stops.FirstOrDefault(t => t.direction_id == direction).trip_headsign;
        }

        public static bool HasMultipleDirection(List<Trip> trips)
        {
            if (trips.Count <= 0)
                return false;

            return trips.Exists(t => t.direction_id != trips[0].direction_id);
        }

        public static bool HasMultipleDirection(List<Stop> stops)
        {
            if (stops.Count <= 0)
                return false;

            return stops.Exists(t => t.direction_id != stops[0].direction_id);
        }

    }
}
