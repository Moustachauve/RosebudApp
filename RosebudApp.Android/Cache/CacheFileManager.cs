using System;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.Util;
using RosebudAppCore.Cache;
using Newtonsoft.Json;
using System.Security.Principal;
using RosebudAppCore.Utils;

namespace RosebudAppAndroid.Cache
{
	public static class CacheFileManager
	{
		private const string LOG_TAG = "RosebudAppCore.Cache";
		private static TimeSpan CacheExpirationTime = new TimeSpan(10, 0, 0);

		public static async Task<T> GetFromFile<T>(string filePath)
		{
			if (!File.Exists(filePath))
				return default(T);

			string json;
			using (var reader = File.OpenText(filePath))
			{
				json = await reader.ReadToEndAsync();
			}

			CacheItem<T> cacheItem;
			try
			{
				cacheItem = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<CacheItem<T>>(json));
			}
			catch (JsonSerializationException ex)
			{
				Log.Warn(LOG_TAG, "Could not deserialize file {0}", filePath);
				Log.Warn(LOG_TAG, ex.ToString());

				//Logging the json if it is not too long to facilitate debugging
				if (json != null && json.Length < 4000)
					Log.Warn(LOG_TAG, json);

				File.Delete(filePath);
				return default(T);
			}

            //We delete cache only if it is expired and we are connected to the internet - Better show expired data than nothing!
			if (cacheItem == null ||
			   cacheItem.CacheExpirationDate < DateTime.Now &&
               !Dependency.NetworkStatusMonitor.CanConnect)
			{
				File.Delete(filePath);
				return default(T);
			}

			return cacheItem.Item;
		}

		public static async Task SaveToFile(string filePath, object item)
		{
			DateTime expirationDate = DateTime.Now.Add(CacheExpirationTime);
			var cacheItem = new CacheItem<object>(item, expirationDate);
			string json = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(cacheItem));
			using (var writer = new StreamWriter(filePath))
			{
				await writer.WriteAsync(json);
			}
		}
	}
}

