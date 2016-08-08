using System;
using System.Collections.Generic;
using Android.Content;
using RosebudAppCore;
using RosebudAppCore.Cache;
using RosebudAppCore.Model;

namespace RosebudAppAndroid.Cache
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

