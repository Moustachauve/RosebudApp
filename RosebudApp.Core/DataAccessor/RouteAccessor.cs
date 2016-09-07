using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RosebudAppCore.Model;
using Newtonsoft.Json;
using RosebudAppCore.Utils;

namespace RosebudAppCore.DataAccessor
{
	public static class RouteAccessor
	{
		const string API_ENDPOINT = "feeds/{0}/routes/";

		public static async Task<List<Route>> GetAllRoutes(int feedId, bool overrideCache)
		{
			List<Route> routes = null;

			if (!overrideCache)
			{
				routes = await Dependency.CacheRepository.RouteCacheManager.GetAllRoutes(feedId);
				if (routes != null)
					return routes;
			}

			routes = await HttpHelper.GetDataFromHttp<List<Route>>(API_ENDPOINT, feedId);

            if (routes == null && overrideCache && !Dependency.NetworkStatusMonitor.CanConnect)
            {
                routes = await Dependency.CacheRepository.RouteCacheManager.GetAllRoutes(feedId);
            }
            else
            {
                await Dependency.CacheRepository.RouteCacheManager.SaveAllRoutes(feedId, routes);
            }

            return routes;
		}

        public static async Task<RouteDetails> GetRouteDetails(int feedId, string routeId, DateTime date, bool overrideCache)
        {
            RouteDetails routeDetails = null;

            if (!overrideCache)
            {
                routeDetails = await Dependency.CacheRepository.RouteCacheManager.GetRouteDetails(feedId, routeId, date);
                if (routeDetails != null)
                    return routeDetails;
            }

            string dateFormatted = TimeFormatter.ToShortDateApi(date);
            string apiUrl = string.Format(API_ENDPOINT, feedId) + "{0}/?date={1}";
            routeDetails = await HttpHelper.GetDataFromHttp<RouteDetails>(apiUrl, routeId, dateFormatted);

            if (routeDetails == null && overrideCache && !Dependency.NetworkStatusMonitor.CanConnect)
            {
                routeDetails = await Dependency.CacheRepository.RouteCacheManager.GetRouteDetails(feedId, routeId, date);
            }
            else
            {
                await Dependency.CacheRepository.RouteCacheManager.SaveRouteDetails(feedId, routeId, date, routeDetails);
            }

            return routeDetails;
        }

        public static async Task<List<Stop>> GetRouteStops(int feedId, string routeId, DateTime date, bool overrideCache)
        {
            List<Stop> routeStops = null;

            if (!overrideCache)
            {
                routeStops = await Dependency.CacheRepository.RouteCacheManager.GetRouteStops(feedId, routeId, date);
                if (routeStops != null)
                    return routeStops;
            }

            string dateFormatted = TimeFormatter.ToShortDateApi(date);
            string apiUrl = string.Format(API_ENDPOINT, feedId) + "{0}/stops?date={1}";
            routeStops = await HttpHelper.GetDataFromHttp<List<Stop>>(apiUrl, routeId, dateFormatted);

            if (routeStops == null && overrideCache && !Dependency.NetworkStatusMonitor.CanConnect)
            {
                routeStops = await Dependency.CacheRepository.RouteCacheManager.GetRouteStops(feedId, routeId, date);
            }
            else
            {
                await Dependency.CacheRepository.RouteCacheManager.SaveRouteStops(feedId, routeId, date, routeStops);
            }

            return routeStops;
        }
    }
}

