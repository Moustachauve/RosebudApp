
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
            string fileName = string.Format("trips-{0}-{1}-{2}.json", feedId, routeId, tripId);
            return await CacheFileManager.GetFromFile<TripDetails>(fileName);
        }

        public async Task SaveStopsForTrip(int feedId, string routeId, string tripId, TripDetails tripDetails)
        {
            string fileName = string.Format("trips-{0}-{1}-{2}.json", feedId, routeId, tripId);
            await CacheFileManager.SaveToFile(fileName, tripDetails);
        }

        public async Task<List<StopTime>> GetStopTimes(int feedId, string routeId, string stopId, DateTime date)
        {
            string fileName = string.Format("stoptimes-{0}-{1}-{2}-{3}.json", feedId, routeId, stopId, TimeFormatter.ToShortDateApi(date));
            return await CacheFileManager.GetFromFile<List<StopTime>>(fileName);
        }

        public async Task SaveStopTimes(int feedId, string routeId, string stopId, DateTime date, List<StopTime> tripDetails)
        {
            string fileName = string.Format("stoptimes-{0}-{1}-{2}-{3}.json", feedId, routeId, stopId, TimeFormatter.ToShortDateApi(date));
            await CacheFileManager.SaveToFile(fileName, tripDetails);
        }
    }
}

