using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyTransitCore.Model;
namespace MyTransitCore.Cache
{
	public interface IFeedCacheManager
	{
		Task<List<Feed>> GetAllFeeds();
		Task SaveAllFeeds(List<Feed> allFeeds);
	}
}

