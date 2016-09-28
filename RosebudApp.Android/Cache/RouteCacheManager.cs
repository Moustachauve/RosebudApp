using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using RosebudAppCore;
using RosebudAppCore.Cache;
using RosebudAppCore.Model;

namespace RosebudAppAndroid.Cache
{
    public class RouteCacheManager : IRouteCacheManager
    {
        Context Context;

        public RouteCacheManager(Context context)
        {
            Context = context;
        }

        public async Task<List<Route>> GetAllRoutes(int feedId)
        {
            string fileName = string.Format("{0}.json", feedId);
            return await CacheFileManager.GetFromFile<List<Route>>(fileName);
        }

        public async Task SaveAllRoutes(int feedId, List<Route> routes)
        {
            string fileName = string.Format("{0}.json", feedId);
            await CacheFileManager.SaveToFile(fileName, routes);
        }

        public async Task<RouteDetails> GetRouteDetails(int feedId, string routeId, DateTime date)
        {
            string fileName = string.Format("{0}-{1}-{2}.json", feedId, routeId, TimeFormatter.ToShortDateApi(date));
            return await CacheFileManager.GetFromFile<RouteDetails>(fileName);
        }

        public async Task SaveRouteDetails(int feedId, string routeId, DateTime date, RouteDetails routeDetails)
        {
            string fileName = string.Format("{0}-{1}-{2}.json", feedId, routeId, TimeFormatter.ToShortDateApi(date));
            await CacheFileManager.SaveToFile(fileName, routeDetails);
        }

        public async Task<List<Stop>> GetRouteStops(int feedId, string routeId, DateTime date)
        {
            string fileName = string.Format("{0}-{1}-{2}-stops.json", feedId, routeId, TimeFormatter.ToShortDateApi(date));
            return await CacheFileManager.GetFromFile<List<Stop>>(fileName);
        }

        public async Task SaveRouteStops(int feedId, string routeId, DateTime date, List<Stop> routeStops)
        {
            string fileName = string.Format("{0}-{1}-{2}-stops.json", feedId, routeId, TimeFormatter.ToShortDateApi(date));
            await CacheFileManager.SaveToFile(fileName, routeStops);
        }
    }
}

