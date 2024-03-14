using System.Net;

namespace Ada_Advanced_Programming_Part_B.Backend.Services
{
    public class SteamApiRequest : ISteamApiRequest
    {
        private readonly string _storeBaseUrl;
        private readonly string _webApiBaseUrl;
        private readonly string _apikey;
        private readonly HttpClientHandler _handler;

        public SteamApiRequest()
        {
            var builder = WebApplication.CreateBuilder();

            _storeBaseUrl = builder.Configuration["Steam:StoreAPI"];
            _webApiBaseUrl = builder.Configuration["Steam:WebAPI"];
            _apikey = builder.Configuration["apikey"];
            _handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
        }

        public async Task<string> GetStoreAsync(string asset, string method, string filter = "")
        {
            return await GetAsync(_storeBaseUrl, asset, method, filter);
        }

        public async Task<string> GetWebApiAsync(string asset, string method, string filter = "")
        {
            return await GetAsync(_webApiBaseUrl, asset, method, filter);
        }

        private async Task<string> GetAsync(string baseUrl, string asset, string method, string filter = "")
        {
            var client = new HttpClient(_handler);
            client.DefaultRequestHeaders.Add("apikey", _apikey);
            var requestUrl = baseUrl + asset + "/" + method + filter;
            return await client.GetStringAsync(requestUrl);
        }
    }
}