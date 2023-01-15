using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
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