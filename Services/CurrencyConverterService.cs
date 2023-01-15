using Newtonsoft.Json;
using tDarkBot.Models;

namespace tDarkBot.Services
{
    public class CurrencyConverterService
    {
        private const string API_KEY = "OAwnMkudwI1ru4te8YYEccrqBiH4iDxoBUBZVIEd";
        private const string BASE_URL = "https://api.freecurrencyapi.com/v1/";

        private IReadOnlyDictionary<string, CurrencyModel> currencies;

        public CurrencyConverterService()
        {
            currencies = new Dictionary<string, CurrencyModel>();
        }

        private async Task<HttpResponseMessage> MakeRequest(string url, HttpMethod method)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(method, BASE_URL + url);

                client.DefaultRequestHeaders.Add("apiKey", API_KEY);
                return await client.SendAsync(request);
            }
        }

        public async Task MakeCacheAsync()
        {
            currencies = await GetCurrenciesAsync();

            Console.WriteLine("{0} currencies has been loaded", currencies.Count);
        }

        public async Task<IReadOnlyDictionary<string, CurrencyModel>> GetCurrenciesAsync()
        {
            using (var message = await MakeRequest("currencies", HttpMethod.Get))
            {
                var json = await message.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<APIResponse<IReadOnlyDictionary<string, CurrencyModel>>>(json);

                if (response == null)
                    throw new InvalidOperationException("The response returns a invalid JSON");

                if (response.Data == null)
                    throw new InvalidOperationException("The API returns invalid data");

                return response.Data;
            }
        }

        public IEnumerable<CurrencyModel> FilterCurrencies(string filter, int max)
        {
            return currencies
            .Where(c => (c.Key + c.Value).ToLower().IndexOf(filter.ToLower()) > -1)
            .Take(25)
            .Select(c => c.Value);
        }

        public bool IsValidCurrency(string code)
        {
            return currencies.ContainsKey(code);
        }

        public CurrencyModel GetCurrencyByCode(string code)
        {
            return currencies[code];
        }

        public IReadOnlyList<CurrencyModel> GetCachedCurrencies()
        {
            return currencies.Values.ToList().AsReadOnly();
        }

        public async Task<decimal> ConvertCurrency(string fromCurrency, string toCurrency, decimal value)
        {
            using (var message = await MakeRequest($"latest?base_currency={fromCurrency}&currencies={toCurrency}", HttpMethod.Get))
            {
                var json = await message.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<APIResponse<IReadOnlyDictionary<string, decimal>>>(json);

                if (response == null)
                    throw new InvalidOperationException("The response returns a invalid JSON");

                if (response.Data == null)
                    throw new InvalidOperationException("The API returns invalid data");

                if (!response.Data.ContainsKey(toCurrency))
                    throw new InvalidOperationException("The response not returns the currency bid!");

                var bid = response.Data[toCurrency];
                return value * bid;
            }
        }
    }
}