using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RosebudAppCore.Model;
namespace RosebudAppCore.Cache
{
	public interface IFeedCacheManager
	{
		Task<List<Feed>> GetAllFeeds();
		Task SaveAllFeeds(List<Feed> allFeeds);
	}
}

