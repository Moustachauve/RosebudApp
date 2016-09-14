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
            if (Context.ExternalCacheDir == null)
                return null;
            
            string fileName = string.Format("{0}.json", feedId);
            string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, fileName);
            return await CacheFileManager.GetFromFile<List<Route>>(path);
        }

        public async Task SaveAllRoutes(int feedId, List<Route> routes)
        {
            if (Context.ExternalCacheDir == null)
                return;

            string fileName = string.Format("{0}.json", feedId);
            string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, fileName);
            await CacheFileManager.SaveToFile(path, routes);
        }

        public async Task<RouteDetails> GetRouteDetails(int feedId, string routeId, DateTime date)
        {
            if (Context.ExternalCacheDir == null)
                return null;

            string fileName = string.Format("{0}-{1}-{2}.json", feedId, routeId, TimeFormatter.ToShortDateApi(date));
            string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, fileName);
            return await CacheFileManager.GetFromFile<RouteDetails>(path);
        }

        public async Task SaveRouteDetails(int feedId, string routeId, DateTime date, RouteDetails routeDetails)
        {
            if (Context.ExternalCacheDir == null)
                return;

            string fileName = string.Format("{0}-{1}-{2}.json", feedId, routeId, TimeFormatter.ToShortDateApi(date));
            string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, fileName);
            await CacheFileManager.SaveToFile(path, routeDetails);
        }

        public async Task<List<Stop>> GetRouteStops(int feedId, string routeId, DateTime date)
        {
            if (Context.ExternalCacheDir == null)
                return null;

            string fileName = string.Format("{0}-{1}-{2}-stops.json", feedId, routeId, TimeFormatter.ToShortDateApi(date));
            string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, fileName);
            return await CacheFileManager.GetFromFile<List<Stop>>(path);
        }

        public async Task SaveRouteStops(int feedId, string routeId, DateTime date, List<Stop> routeStops)
        {
            if (Context.ExternalCacheDir == null)
                return;

            string fileName = string.Format("{0}-{1}-{2}-stops.json", feedId, routeId, TimeFormatter.ToShortDateApi(date));
            string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, fileName);
            await CacheFileManager.SaveToFile(path, routeStops);
        }
    }
}

