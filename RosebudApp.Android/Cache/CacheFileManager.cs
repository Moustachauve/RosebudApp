using System;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.Util;
using RosebudAppCore.Cache;
using Newtonsoft.Json;
using System.Security.Principal;
using RosebudAppCore.Utils;
using RosebudAppCore.DataAccessor;

namespace RosebudAppAndroid.Cache
{
    public static class CacheFileManager
    {
        const string LOG_TAG = "RosebudAppCore.Cache";

#if DEBUG
        static TimeSpan CacheExpirationTime = new TimeSpan(0, 0, 5);
#else
        static TimeSpan CacheExpirationTime = new TimeSpan(10, 0, 0);
#endif

        public static async Task<T> GetFromFile<T>(string fileName)
        {
            if (Dependency.PathHelper.TempFolderPath == null)
                return default(T);

            string path = Path.Combine(Dependency.PathHelper.TempFolderPath, fileName);

            if (!File.Exists(path))
                return default(T);

            CacheItem<T> cacheItem;
            try
            {
                cacheItem = await LocalFileHelper.GetFromFile<CacheItem<T>>(path);
            }
            catch (Exception ex) when (ex is JsonSerializationException || ex is JsonReaderException)
            {
                Log.Warn(LOG_TAG, "Could not deserialize file {0}", fileName);
                Log.Warn(LOG_TAG, ex.ToString());

                File.Delete(path);
                return default(T);
            }

            //We delete cache only if it is created in the future or expired and we are connected to the internet - Better show expired data than nothing!
            if (cacheItem == null ||
               (cacheItem.CacheCreationDate > DateTime.Now || 
               cacheItem.CacheCreationDate < cacheItem.CacheCreationDate.Add(CacheExpirationTime)/* &&
               Dependency.NetworkStatusMonitor.CanConnect*/))
            {
                File.Delete(path);
                return default(T);
            }

            return cacheItem.Item;
        }

        public static async Task SaveToFile(string fileName, object item)
        {
            if (Dependency.PathHelper.TempFolderPath == null)
                return;

            string path = Path.Combine(Dependency.PathHelper.TempFolderPath, fileName);

            var cacheItem = new CacheItem<object>(item);

            await LocalFileHelper.SaveToFile(path, cacheItem);
        }
    }
}

