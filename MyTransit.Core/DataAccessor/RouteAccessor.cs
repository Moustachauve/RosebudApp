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
			List<Route> routes = await HttpHelper.CacheRepository.RouteCacheManager.GetAllRoutes(feedId);
			if (routes != null)
				return routes;
			
			routes = await HttpHelper.GetDataFromHttp<List<Route>>(API_ENDPOINT, feedId);
			await HttpHelper.CacheRepository.RouteCacheManager.SaveAllRoutes(feedId, routes);

			return routes;
		}

		public static async Task<RouteDetails> GetRouteDetails(int feedId, string routeId, DateTime date)
		{
			RouteDetails routeDetails = await HttpHelper.CacheRepository.RouteCacheManager.GetRouteDetails(feedId, routeId, date);
			if (routeDetails != null)
				return routeDetails;

			string dateFormatted = TimeFormatter.ToShortDateApi(date);
			string apiUrl = string.Format(API_ENDPOINT, feedId) + "{0}/?date={1}";
			routeDetails =  await HttpHelper.GetDataFromHttp<RouteDetails>(apiUrl, routeId, dateFormatted);
			await HttpHelper.CacheRepository.RouteCacheManager.SaveRouteDetails(feedId, routeId, date, routeDetails);

			return routeDetails;
		}
	}
}

