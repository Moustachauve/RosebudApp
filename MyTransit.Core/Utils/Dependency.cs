using MyTransit.Core.Cache;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyTransit.Core.Utils
{
    public static class Dependency
    {
        public static AbstractCacheRepository CacheRepository { get; set; }
        public static INetworkStatusMonitor NetworkStatusMonitor { get; set; }
    }
}
