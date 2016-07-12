using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyTransit.Core.Model;
using Newtonsoft.Json;

namespace MyTransit.Core.DataAccessor
{
	public static class FeedAccessor
	{
		const string API_ENDPOINT = "feeds/";

		public static async Task<List<Feed>> GetAllFeeds()
		{
			return await HttpHelper.GetDataFromHttp<List<Feed>>(API_ENDPOINT);
		}
	}
}

