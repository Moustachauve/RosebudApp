
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
	public class FeedCacheManager : IFeedCacheManager
	{
		private const string FEED_LIST_FILENAME = "feeds.json";
		private Context Context;

		public FeedCacheManager(Context context)
		{
			Context = context;
		}

		public async Task<List<Feed>> GetAllFeeds()
		{
			string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, FEED_LIST_FILENAME);
			return await CacheFileManager.GetFromFile<List<Feed>>(path);
		}

		public async Task SaveAllFeeds(List<Feed> allFeeds)
		{
			string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, FEED_LIST_FILENAME);
			await CacheFileManager.SaveToFile(path, allFeeds);
		}
	}
}

