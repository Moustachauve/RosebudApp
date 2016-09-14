
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
            if (Context.ExternalCacheDir == null)
                return null;
            
            string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, FEED_LIST_FILENAME);
            return await CacheFileManager.GetFromFile<List<Feed>>(path);
        }

        public async Task SaveAllFeeds(List<Feed> allFeeds)
        {
            if (Context.ExternalCacheDir == null)
                return;
            
            string path = Path.Combine(Context.ExternalCacheDir.AbsolutePath, FEED_LIST_FILENAME);
            await CacheFileManager.SaveToFile(path, allFeeds);
        }
    }
}

