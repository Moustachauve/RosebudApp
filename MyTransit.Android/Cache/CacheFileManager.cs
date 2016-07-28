using System;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.Util;
using MyTransit.Core.Cache;
using Newtonsoft.Json;
using System.Security.Principal;

namespace MyTransit.Android.Cache
{
	public static class CacheFileManager
	{
		private const string TAG_LOG = "MyTransit.Cache";
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
				Log.Warn(TAG_LOG, "Could not deserialize file " + filePath);
				Log.Warn(TAG_LOG, ex.ToString());

				//Logging the json if it is not too long to facilitate debugging
				if (json != null && json.Length < 4000)
					Log.Warn(TAG_LOG, json);

				File.Delete(filePath);
				return default(T);
			}

			if (cacheItem == null ||
			   cacheItem.CacheExpirationDate < DateTime.Now)
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
			using (var writer = File.CreateText(filePath))
			{
				await writer.WriteAsync(json);
			}
		}
	}
}

