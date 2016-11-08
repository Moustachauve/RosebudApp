using System;
using System.Threading.Tasks;
using RosebudAppCore.Model;
using System.Collections.Generic;
using RosebudAppCore.Model.Enum;

namespace RosebudAppCore.Cache
{
	public interface IStopCacheManager
	{
        Task<TripDetails> GetStopsForTrip(int feedId, string routeId, string tripId);
        Task SaveStopsForTrip(int feedId, string routeId, string tripId, TripDetails tripDetails);

        Task<List<StopTime>> GetStopTimes(int feedId, string routeId, string stopId, TripDirection directionId, DateTime date);
        Task SaveStopTimes(int feedId, string routeId, string stopId, TripDirection directionId, DateTime date, List<StopTime> tripDetails);

    }
}

