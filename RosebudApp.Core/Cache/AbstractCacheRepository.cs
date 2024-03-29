﻿using System;
namespace RosebudAppCore.Cache
{
    public abstract class AbstractCacheRepository
    {
        IFeedCacheManager feedCacheManager;
        public IFeedCacheManager FeedCacheManager
        {
            get
            {
                if (feedCacheManager == null)
                {
                    feedCacheManager = CreateFeedCacheManager();
                }
                return feedCacheManager;
            }
        }

        IRouteCacheManager routeCacheManager;
        public IRouteCacheManager RouteCacheManager
        {
            get
            {
                if (routeCacheManager == null)
                {
                    routeCacheManager = CreateRouteCacheManager();
                }
                return routeCacheManager;
            }
        }

        IStopCacheManager stopCacheManager;
        public IStopCacheManager StopCacheManager
        {
            get
            {
                if (stopCacheManager == null)
                {
                    stopCacheManager = CreateStopCacheManager();
                }
                return stopCacheManager;
            }
        }


        protected abstract IFeedCacheManager CreateFeedCacheManager();
        protected abstract IRouteCacheManager CreateRouteCacheManager();
        protected abstract IStopCacheManager CreateStopCacheManager();
    }
}

