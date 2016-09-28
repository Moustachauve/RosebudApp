
using System;
using System.Collections.Generic;
using RosebudAppCore.Cache;
using RosebudAppCore.Model;
using Android.Content;
using System.IO;
using Newtonsoft.Json;
using Android.Util;
using System.Threading.Tasks;

namespace RosebudAppAndroid.Cache
{
    public class FeedCacheManager : IFeedCacheManager
    {
        const string FEED_LIST_FILENAME = "feeds.json";
        Context Context;

        public FeedCacheManager(Context context)
        {
            Context = context;
        }

        public async Task<List<Feed>> GetAllFeeds()
        {
            return await CacheFileManager.GetFromFile<List<Feed>>(FEED_LIST_FILENAME);
        }

        public async Task SaveAllFeeds(List<Feed> allFeeds)
        {
            await CacheFileManager.SaveToFile(FEED_LIST_FILENAME, allFeeds);
        }
    }
}

