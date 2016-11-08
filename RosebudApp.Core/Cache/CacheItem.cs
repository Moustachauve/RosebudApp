using System;
namespace RosebudAppCore.Cache
{
	public class CacheItem<T>
	{
		public T Item { get; set; }
		public DateTime CacheCreationDate { get; set; }

		public CacheItem()
		{
		}

		public CacheItem(T item)
		{
			Item = item;
			CacheCreationDate = DateTime.Now;
		}
	}
}

