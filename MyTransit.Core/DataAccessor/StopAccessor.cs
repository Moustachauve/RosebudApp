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

		public static async Task<TripDetails> GetTripsForRoute(int feedId, string routeId, string tripId)
		{

			using (var client = HttpHelper.GetHttpClient(API_ENDPOINT, feedId, routeId, tripId))
			{
				var test = await client.GetAsync("stops");
				string debug = await test.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<TripDetails>(debug);
			}
		}
	}
}

