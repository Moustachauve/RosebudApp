using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyTransit.Core.DataAccessor
{
	public static class HttpHelper
	{
		//public const string API_URL = "http://localhost:1337/";
		public const string API_URL = "http://10.0.2.2:1337/";		//Emulator

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
	}
}

