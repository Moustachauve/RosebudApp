using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyTransit.Core.Model;
using Newtonsoft.Json;

namespace MyTransit.Core.DataAccessor
{
	public static class FeedAccessor
	{
		const string API_ENDPOINT = "feeds/";

		public static async Task<List<Feed>> GetAllFeeds()
		{
			List<Feed> feeds = await HttpHelper.CacheRepository.FeedCacheManager.GetAllFeeds();
			if (feeds != null)
			{
				return feeds;
			}

			feeds = await HttpHelper.GetDataFromHttp<List<Feed>>(API_ENDPOINT);
			await HttpHelper.CacheRepository.FeedCacheManager.SaveAllFeeds(feeds);

			return feeds;
		}
	}
}

