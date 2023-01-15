using Newtonsoft.Json;
using tDarkBot.Converters;

namespace tDarkBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var instance = new Program();
            instance.Handle().Wait();
        }

        private DarkBot bot;

        private Program()
        {
            bot = new DarkBot();

            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new CurrencyModelConverter());

                return settings;
            };
        }

        private async Task Handle()
        {
            await bot.Start();
        }
    }
}