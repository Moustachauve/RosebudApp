using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RosebudAppCore.DataAccessor;
using RosebudAppCore.Model;
using Newtonsoft.Json;
using RosebudAppCore.Utils;

namespace RosebudAppCore.DataAccessor
{
	public static class StopAccessor
	{
		const string API_ENDPOINT = "feeds/{0}/routes/{1}/trips/{2}/";

		public static async Task<TripDetails> GetStopsForTrip(int feedId, string routeId, string tripId, bool overrideCache)
		{
			TripDetails tripDetails = null;
			if (!overrideCache)
			{
				tripDetails = await Dependency.CacheRepository.StopCacheManager.GetStopsForTrip(feedId, routeId, tripId);
				if (tripDetails != null)
					return tripDetails;
			}

			string apiUrl = string.Format(API_ENDPOINT, feedId, routeId, tripId) + "stops";
			tripDetails = await HttpHelper.GetDataFromHttp<TripDetails>(apiUrl);

            if (tripDetails == null && overrideCache && !Dependency.NetworkStatusMonitor.CanConnect)
            {
                tripDetails = await Dependency.CacheRepository.StopCacheManager.GetStopsForTrip(feedId, routeId, tripId);
            }
            else
            {
                await Dependency.CacheRepository.StopCacheManager.SaveStopsForTrip(feedId, routeId, tripId, tripDetails);
            }

            return tripDetails;
		}
	}
}

