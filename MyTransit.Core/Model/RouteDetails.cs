using System;
using System.Collections.Generic;
using MyTransit.Core.Model;
using System.Linq;
namespace MyTransit.Core
{
	public class RouteDetails
	{
		public List<Trip> trips { get; set; }

		public List<Trip> GetTripsForDirection(TripDirection direction)
		{
			if (direction == TripDirection.AnyDirection)
				return trips;

			return trips.Where(t => t.direction_id == direction).ToList();
		}

		public string GetDirectionName(TripDirection direction)
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

		public bool HasMultipleDirection()
		{
			if (trips.Count <= 0)
				return false;

			return trips.Exists(t => t.direction_id != trips[0].direction_id);
		}
	}
}

