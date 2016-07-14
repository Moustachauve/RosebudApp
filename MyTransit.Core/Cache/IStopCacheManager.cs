using System;
using System.Threading.Tasks;
using MyTransit.Core.Model;

namespace MyTransit.Core.Cache
{
	public interface IStopCacheManager
	{
		Task<TripDetails> GetStopsForTrip(int feedId, string routeId, string tripId); 
		Task SaveStopsForTrip(int feedId, string routeId, string tripId, TripDetails tripDetails);
	}
}

