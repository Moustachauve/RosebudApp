using RosebudAppCore.Cache;
using System;
using System.Collections.Generic;
using System.Text;

namespace RosebudAppCore.Utils
{
    public static class Dependency
    {
        public static AbstractCacheRepository CacheRepository { get; set; }
        public static INetworkStatusMonitor NetworkStatusMonitor { get; set; }
        public static IPreferenceManager PreferenceManager { get; set; }
    }
}
