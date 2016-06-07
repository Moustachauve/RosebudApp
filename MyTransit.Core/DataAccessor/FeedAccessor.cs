using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyTransit.Core
{
	public static class FeedAccessor
	{
		const string API_ENDPOINT = "feeds/";

		public static async Task<List<Feed>> GetAllFeeds() {

			using(var client = HttpHelper.GetHttpClient(API_ENDPOINT)) {
				var test = await client.GetAsync("");
				return JsonConvert.DeserializeObject<List<Feed>>(await test.Content.ReadAsStringAsync());
			}
		}
	}
}

