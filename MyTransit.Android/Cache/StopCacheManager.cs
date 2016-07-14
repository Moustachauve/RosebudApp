
using System;
using System.Collections.Generic;
using MyTransit.Core.Cache;
using MyTransit.Core.Model;
using Android.Content;
using System.IO;
using Newtonsoft.Json;
using Android.Util;
using System.Threading.Tasks;

namespace MyTransit.Android
{
	public class StopCacheManager : IStopCacheManager
	{
		private Context Context;

		public StopCacheManager(Context context)
		{
			Context = context;
		}

		public async Task<TripDetails> GetStopsForTrip(int feedId, string routeId, string tripId)
		{
			string fileName = string.Format("trips-{0}-{1}-{2}.json", feedId, routeId, tripId);
			string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, fileName);
			return await CacheFileManager.GetFromFile<TripDetails>(path);
		}

		public async Task SaveStopsForTrip(int feedId, string routeId, string tripId, TripDetails tripDetails)
		{
			string fileName = string.Format("trips-{0}-{1}-{2}.json", feedId, routeId, tripId);
			string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, fileName);
			await CacheFileManager.SaveToFile(path, tripDetails);
		}
	}
}

