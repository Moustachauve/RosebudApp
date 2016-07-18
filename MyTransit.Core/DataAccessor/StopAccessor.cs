using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyTransit.Core.DataAccessor;
using MyTransit.Core.Model;
using Newtonsoft.Json;

namespace MyTransit.Core.DataAccessor
{
	public static class StopAccessor
	{
		const string API_ENDPOINT = "feeds/{0}/routes/{1}/trips/{2}/";

		public static async Task<TripDetails> GetStopsForTrip(int feedId, string routeId, string tripId, bool overrideCache)
		{
			TripDetails tripDetails = null;
			if (!overrideCache)
			{
				tripDetails = await HttpHelper.CacheRepository.StopCacheManager.GetStopsForTrip(feedId, routeId, tripId);
				if (tripDetails != null)
					return tripDetails;
			}

			string apiUrl = string.Format(API_ENDPOINT, feedId, routeId, tripId) + "stops";
			tripDetails = await HttpHelper.GetDataFromHttp<TripDetails>(apiUrl);
			await HttpHelper.CacheRepository.StopCacheManager.SaveStopsForTrip(feedId, routeId, tripId, tripDetails);

			return tripDetails;
		}
	}
}

