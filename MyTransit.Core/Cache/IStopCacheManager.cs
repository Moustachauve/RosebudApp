using System;
using System.Threading.Tasks;
using RosebudAppCore.Model;

namespace RosebudAppCore.Cache
{
	public interface IStopCacheManager
	{
		Task<TripDetails> GetStopsForTrip(int feedId, string routeId, string tripId); 
		Task SaveStopsForTrip(int feedId, string routeId, string tripId, TripDetails tripDetails);
	}
}

