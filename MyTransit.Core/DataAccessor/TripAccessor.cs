using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyTransit.Core.DataAccessor;
using MyTransit.Core.Model;
using Newtonsoft.Json;
using System.Net.Http;

namespace MyTransit.Core.DataAccessor
{
	public class TripAccessor
	{
		const string API_ENDPOINT = "feeds/{0}/routes/{1}/";

		[Obsolete("TripAccessor.GetTripsForRoute is obsolete, please use RouteAccessor.GetRouteDetails instead")]
		public static async Task<RouteDetails> GetTripsForRoute(int feedId, string routeId, DateTime date)
		{
			string dateFormatted = TimeFormatter.ToShortDateApi(date);
			string apiUrl = string.Format(API_ENDPOINT, feedId, routeId) + "trips?date={0}";
			return await HttpHelper.GetDataFromHttp<RouteDetails>(apiUrl, dateFormatted);
		}
	}
}
