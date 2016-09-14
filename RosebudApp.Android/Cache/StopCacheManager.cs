
using System;
using System.Collections.Generic;
using RosebudAppCore.Cache;
using RosebudAppCore.Model;
using Android.Content;
using System.IO;
using Newtonsoft.Json;
using Android.Util;
using System.Threading.Tasks;
using RosebudAppCore;

namespace RosebudAppAndroid.Cache
{
    public class StopCacheManager : IStopCacheManager
    {
        Context Context;

        public StopCacheManager(Context context)
        {
            Context = context;
        }

        public async Task<TripDetails> GetStopsForTrip(int feedId, string routeId, string tripId)
        {
            if (Context.ExternalCacheDir == null)
                return null;

            string fileName = string.Format("trips-{0}-{1}-{2}.json", feedId, routeId, tripId);
            string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, fileName);
            return await CacheFileManager.GetFromFile<TripDetails>(path);
        }

        public async Task SaveStopsForTrip(int feedId, string routeId, string tripId, TripDetails tripDetails)
        {
            if (Context.ExternalCacheDir == null)
                return;

            string fileName = string.Format("trips-{0}-{1}-{2}.json", feedId, routeId, tripId);
            string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, fileName);
            await CacheFileManager.SaveToFile(path, tripDetails);
        }

        public async Task<List<StopTime>> GetStopTimes(int feedId, string routeId, string stopId, DateTime date)
        {
            if (Context.ExternalCacheDir == null)
                return null;

            string fileName = string.Format("stoptimes-{0}-{1}-{2}-{3}.json", feedId, routeId, stopId, TimeFormatter.ToShortDateApi(date));
            string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, fileName);
            return await CacheFileManager.GetFromFile<List<StopTime>>(path);
        }

        public async Task SaveStopTimes(int feedId, string routeId, string stopId, DateTime date, List<StopTime> tripDetails)
        {
            if (Context.ExternalCacheDir == null)
                return;

            string fileName = string.Format("stoptimes-{0}-{1}-{2}-{3}.json", feedId, routeId, stopId, TimeFormatter.ToShortDateApi(date));
            string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, fileName);
            await CacheFileManager.SaveToFile(path, tripDetails);
        }
    }
}

