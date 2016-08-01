using System;
namespace MyTransitCore.Cache
{
	public class CacheItem<T>
	{
		public T Item { get; set; }
		public DateTime CacheExpirationDate { get; set; }

		public CacheItem()
		{
		}

		public CacheItem(T item, DateTime expiration)
		{
			Item = item;
			CacheExpirationDate = expiration;
		}
	}
}

