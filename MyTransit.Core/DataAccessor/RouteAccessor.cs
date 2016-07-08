﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyTransit.Core.Model;
using Newtonsoft.Json;

namespace MyTransit.Core.DataAccessor
{
	public static class RouteAccessor
	{
		const string API_ENDPOINT = "feeds/{0}/routes/";

		public static async Task<List<Route>> GetAllRoutes(int feedId)
		{
			using (var client = HttpHelper.GetHttpClient(API_ENDPOINT, feedId))
			{
				var test = await client.GetAsync("");
				string debug = await test.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<List<Route>>(debug);
			}
		}

		public static async Task<RouteDetails> GetRouteDetails(int feedId, string routeId, DateTime date)
		{
			string dateFormatted = TimeFormatter.ToShortDateApi(date);
			using (var client = HttpHelper.GetHttpClient(API_ENDPOINT, feedId))
			{
				var request = await client.GetAsync(routeId + "/?date=" + dateFormatted);
				string jsonData = await request.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<RouteDetails>(jsonData);
			}
		}
	}
}

