using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyTransit.Core.DataAccessor;
using MyTransit.Core.Model;
using Newtonsoft.Json;

namespace MyTransit.Core.DataAccessor
{
	public class TripAccessor
	{
		const string API_ENDPOINT = "feeds/{0}/routes/{1}/";

		public static async Task<List<Trip>> GetTripsForRoute(int feedId, string routeId)
		{

			using (var client = HttpHelper.GetHttpClient(API_ENDPOINT, feedId, routeId))
			{
				var test = await client.GetAsync("trips");
				string debug = await test.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<List<Trip>>(debug);
			}
		}
	}
}

