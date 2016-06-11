using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyTransit.Core
{
	public static class RouteAccessor
	{
		const string API_ENDPOINT = "routes/";

		public static async Task<List<Route>> GetAllRoutes(int feedId) {

			using(var client = HttpHelper.GetHttpClient(API_ENDPOINT)) {
				var test = await client.GetAsync(feedId.ToString());
				string debug = await test.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<List<Route>>(debug);
			}
		}
	}
}

