﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using MyTransit.Core;
using MyTransit.Core.Cache;
using MyTransit.Core.Model;

namespace MyTransit.Android
{
	public class RouteCacheManager : IRouteCacheManager
	{
		private Context Context;

		public RouteCacheManager(Context context)
		{
			Context = context;
		}

		public async Task<List<Route>> GetAllRoutes(int feedId)
		{
			string fileName = string.Format("{0}.json", feedId);
			string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, fileName);
			return await CacheFileManager.GetFromFile<List<Route>>(path);
		}

		public async Task SaveAllRoutes(int feedId, List<Route> routes)
		{
			string fileName = string.Format("{0}.json", feedId);
			string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, fileName);
			await CacheFileManager.SaveToFile(path, routes);
		}

		public async Task<RouteDetails> GetRouteDetails(int feedId, string routeId, DateTime date)
		{
			string fileName = string.Format("{0}-{1}-{2}.json", feedId, routeId, TimeFormatter.ToShortDateApi(date));
			string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, fileName);
			return await CacheFileManager.GetFromFile<RouteDetails>(path);
		}

		public async Task SaveRouteDetails(int feedId, string routeId, DateTime date, RouteDetails routeDetails)
		{
			string fileName = string.Format("{0}-{1}-{2}.json", feedId, routeId, TimeFormatter.ToShortDateApi(date));
			string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, fileName);
			await CacheFileManager.SaveToFile(path, routeDetails);
		}
	}
}

