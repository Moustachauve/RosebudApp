using MyTransitCore.Cache;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyTransitCore.Utils
{
    public static class Dependency
    {
        public static AbstractCacheRepository CacheRepository { get; set; }
        public static INetworkStatusMonitor NetworkStatusMonitor { get; set; }
    }
}
