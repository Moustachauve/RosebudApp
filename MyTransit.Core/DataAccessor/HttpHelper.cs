using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Android.Util;
using MyTransit.Core.Cache;

namespace MyTransit.Core.DataAccessor
{
	public static class HttpHelper
	{
		private const string LOG_TAG = "MyTransit.HttpHelper";

		public const string API_URL = "http://10.0.2.2:1337/";
		//public const string API_URL = "http://cgagnier.ca:1337/";

		public static AbstractCacheRepository CacheRepository { get; set; }

		public static HttpClient GetHttpClient()
		{
			return GetHttpClient(null);
		}

		public static HttpClient GetHttpClient(string apiEndpoint)
		{
			if (string.IsNullOrEmpty(apiEndpoint))
				apiEndpoint = "";

			var client = new HttpClient();
			client.BaseAddress = new Uri(API_URL + apiEndpoint);
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			return client;
		}

		public static HttpClient GetHttpClient(string apiEndpoint, params object[] apiParameters)
		{
			return GetHttpClient(string.Format(apiEndpoint, apiParameters));
		}

		public static async Task<T> ExecuteHttpRequest<T>(HttpClient httpClient, string url)
		{
			HttpResponseMessage request = await httpClient.GetAsync(url);
			string jsonData = await request.Content.ReadAsStringAsync();
			Log.Info(LOG_TAG, "Request {0} done: {1} octets", request.RequestMessage.RequestUri, request.Content.Headers.ContentLength);
			return JsonConvert.DeserializeObject<T>(jsonData);
		}

		public static async Task<T> ExecuteHttpRequest<T>(HttpClient httpClient, string url, params object[] parameters)
		{
			return await ExecuteHttpRequest<T>(httpClient, string.Format(url, parameters));
		}

		public static async Task<T> GetDataFromHttp<T>(string url)
		{
			using (HttpClient httpClient = GetHttpClient(url))
			{
				return await ExecuteHttpRequest<T>(httpClient, "");
			}
		}

		public static async Task<T> GetDataFromHttp<T>(string url, params object[] parameters)
		{
			return await GetDataFromHttp<T>(string.Format(url, parameters));
		}
	}
}

