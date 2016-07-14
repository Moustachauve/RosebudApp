using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyTransit.Core.Model;
namespace MyTransit.Core.Cache
{
	public interface IFeedCacheManager
	{
		Task<List<Feed>> GetAllFeeds();
		Task SaveAllFeeds(List<Feed> allFeeds);
	}
}

