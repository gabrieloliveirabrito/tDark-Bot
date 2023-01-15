using tDarkBot.Models;

namespace tDarkBot.Services
{
    public class CurrencyConverterService
    {
        private const string API_KEY = "OAwnMkudwI1ru4te8YYEccrqBiH4iDxoBUBZVIEd";
        private const string BASE_URL = "https://api.freecurrencyapi.com/v1/";

        private IReadOnlyList<CurrencyModel> currencies;

        public CurrencyConverterService()
        {
            currencies = new List<CurrencyModel>();
        }

        private async Task<HttpResponseMessage> MakeRequest(string url, HttpMethod method)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(method, BASE_URL + url);

                client.DefaultRequestHeaders.Add("apikey", API_KEY);
                return await client.SendAsync(request);
            }
        }

        public async Task MakeCacheAsync()
        {
            currencies = await GetCurrenciesAsync();
        }

        public async Task<IReadOnlyCollection<CurrencyModel>> GetCurrenciesAsync()
        {

        }
    }
}