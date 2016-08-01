using System;
using System.Collections.Generic;
using Android.Content;
using MyTransitCore;
using MyTransitCore.Cache;
using MyTransitCore.Model;

namespace MyTransitAndroid.Cache
{
	public class CacheRepository : AbstractCacheRepository
	{
		private Context Context;

		public CacheRepository(Context context)
		{
			Context = context;

		}

		protected override IFeedCacheManager CreateFeedCacheManager()
		{
			return new FeedCacheManager(Context);
		}

		protected override IRouteCacheManager CreateRouteCacheManager()
		{
			return new RouteCacheManager(Context);
		}

		protected override IStopCacheManager CreateStopCacheManager()
		{
			return new StopCacheManager(Context);
		}
	}
}

