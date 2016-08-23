using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Android.Util;
using System.Net;
using RosebudAppCore.Utils;

namespace RosebudAppCore.DataAccessor
{
    public static class HttpHelper
    {
        private const string LOG_TAG = "RosebudAppCore.HttpHelper";

        //public const string API_URL = "http://10.0.2.2:1337/";
        public const string API_URL = "http://192.168.0.40:1337/";
        //public const string API_URL = "http://cgagnier.ca:1337/";

        public static event Action ServerErrorOccured;

        public static HttpClient GetHttpClient()
        {
            return GetHttpClient(null);
        }

        public static HttpClient GetHttpClient(string apiEndpoint)
        {
            if (string.IsNullOrEmpty(apiEndpoint))
                apiEndpoint = "";

            HttpClientHandler handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.GZip;

            var client = new HttpClient(handler);
            client.BaseAddress = new Uri(API_URL + apiEndpoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            return client;
        }

        public static HttpClient GetHttpClient(string apiEndpoint, params object[] apiParameters)
        {
            return GetHttpClient(string.Format(apiEndpoint, apiParameters));
        }

        public static async Task<T> ExecuteHttpRequest<T>(HttpClient httpClient, string url)
        {
            if (!Dependency.NetworkStatusMonitor.CanConnect)
            {
                return default(T);
            }

            HttpResponseMessage request = null;

            try
            {
                request = await httpClient.GetAsync(url);
            }
            catch (Exception ex)
            {
                ServerErrorOccured?.Invoke();
                Log.Error(LOG_TAG, "Cannot contact server ({0}/{1}). Exception: {2}", httpClient.BaseAddress, url, ex);
                return default(T);
            }

            if (request.StatusCode != HttpStatusCode.OK)
            {
                ServerErrorOccured?.Invoke();
                Log.Error(LOG_TAG, "Error \"{0}\" ({1}) while resquesting {2} from server", request.StatusCode, (int)request.StatusCode, request.RequestMessage.RequestUri);
                return default(T);
            }

            string jsonData = await request.Content.ReadAsStringAsync();
            Log.Info(LOG_TAG, "Request {0} done: {1} octets", request.RequestMessage.RequestUri, request.Content.Headers.ContentLength);

            T result = default(T);
            try
            {
                result = JsonConvert.DeserializeObject<T>(jsonData);
            }
            catch (JsonSerializationException ex)
            {
                ServerErrorOccured?.Invoke();

                Log.Warn(LOG_TAG, "Could not deserialize request {0}", request.RequestMessage.RequestUri);
                Log.Warn(LOG_TAG, ex.ToString());

                //Logging the json if it is not too long to facilitate debugging
                if (jsonData != null && jsonData.Length < 4000)
                {
                    Log.Error(LOG_TAG, jsonData);
                }
            }

            return result;
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

