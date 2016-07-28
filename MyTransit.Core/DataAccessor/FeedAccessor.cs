using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyTransit.Core.Model;
using Newtonsoft.Json;
using MyTransit.Core.Utils;

namespace MyTransit.Core.DataAccessor
{
	public static class FeedAccessor
	{
		const string API_ENDPOINT = "feeds/";

		public static async Task<List<Feed>> GetAllFeeds(bool overrideCache)
		{
			List<Feed> feeds;
			if (!overrideCache)
			{
				feeds = await Dependency.CacheRepository.FeedCacheManager.GetAllFeeds();
				if (feeds != null)
				{
					return feeds;
				}
			}

			feeds = await HttpHelper.GetDataFromHttp<List<Feed>>(API_ENDPOINT);

            if(feeds == null && overrideCache && Dependency.NetworkStatusMonitor.State == NetworkState.Disconnected)
            {
                feeds = await Dependency.CacheRepository.FeedCacheManager.GetAllFeeds();
            }
            else
            {
                await Dependency.CacheRepository.FeedCacheManager.SaveAllFeeds(feeds);
            }

            return feeds;
		}
	}
}

