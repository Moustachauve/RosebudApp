using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyTransit.Core.Model;
using Newtonsoft.Json;

namespace MyTransit.Core.DataAccessor
{
	public static class RouteAccessor
	{
		const string API_ENDPOINT = "feeds/{0}/routes/";

		public static async Task<List<Route>> GetAllRoutes(int feedId)
		{
			return await HttpHelper.GetDataFromHttp<List<Route>>(API_ENDPOINT, feedId);
		}

		public static async Task<RouteDetails> GetRouteDetails(int feedId, string routeId, DateTime date)
		{
			string dateFormatted = TimeFormatter.ToShortDateApi(date);
			string apiUrl = string.Format(API_ENDPOINT, feedId) + "{0}/?date={1}";
			return await HttpHelper.GetDataFromHttp<RouteDetails>(apiUrl, routeId, dateFormatted);
		}
	}
}

