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

		public static async Task<RouteDetails> GetTripsForRoute(int feedId, string routeId, DateTime date)
		{
			string dateFormatted = TimeFormatter.ToShortDateApi(date);
			using (var client = HttpHelper.GetHttpClient(API_ENDPOINT, feedId, routeId))
			{
				var request = await client.GetAsync("trips?date=" + dateFormatted);
				string jsonData = await request.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<RouteDetails>(jsonData);
			}
		}
	}
}

