using System;
using System.Threading.Tasks;
using RosebudAppCore.Model;
using System.Collections.Generic;

namespace RosebudAppCore.Cache
{
	public interface IStopCacheManager
	{
        Task<TripDetails> GetStopsForTrip(int feedId, string routeId, string tripId);
        Task SaveStopsForTrip(int feedId, string routeId, string tripId, TripDetails tripDetails);

        Task<List<StopTime>> GetStopTimes(int feedId, string routeId, string stopId, DateTime date);
        Task SaveStopTimes(int feedId, string routeId, string stopId, DateTime date, List<StopTime> tripDetails);

    }
}

