using System.Linq;
using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using tDarkBot.Models;
using tDarkBot.Services;
using tDarkBot.TypeConverters;

namespace tDarkBot
{
    public class DarkBot
    {
        private IServiceProvider services;
        private DiscordSocketClient client;
        private InteractionService interaction;
        private CurrencyConverterService currencyConverter;
        private CommandHandlingService commandHandling;
        private TimeService _timeService;

        public DarkBot()
        {
            services = ConfigureServices();

            client = services.GetRequiredService<DiscordSocketClient>();
            interaction = services.GetRequiredService<InteractionService>();
            currencyConverter = services.GetRequiredService<CurrencyConverterService>();
            commandHandling = services.GetRequiredService<CommandHandlingService>();
            _timeService = services.GetRequiredService<TimeService>();
        }

        private IServiceProvider ConfigureServices()
        {
            var builder = new ServiceCollection()
            .AddScoped(MakeHttpClient)
            .AddSingleton(new CurrencyConverterService())
            .AddSingleton(MakeClientConfig)
            .AddSingleton(MakeClient)
            .AddSingleton(MakeInteractionConfig)
            .AddSingleton(MakeInteraction)
            .AddSingleton(MakeCommandService)
            .AddSingleton(MakeTimeService);

            return builder.BuildServiceProvider();
        }

        private HttpClient MakeHttpClient(IServiceProvider provider)
        {
            var client = new HttpClient();
            return client;
        }

        private DiscordSocketConfig MakeClientConfig(IServiceProvider provider)
        {
            var config = new DiscordSocketConfig();
            config.GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.MessageContent | GatewayIntents.Guilds | GatewayIntents.GuildMessages;
            config.UseInteractionSnowflakeDate = true;
            config.UseSystemClock = true;
            config.AlwaysDownloadUsers = true;
            config.AlwaysDownloadDefaultStickers = true;
            config.AlwaysResolveStickers = true;
            config.APIOnRestInteractionCreation = true;
            config.MessageCacheSize = 500;

            var debugVar = Environment.GetEnvironmentVariable("BOT_DEBUG");
            if (debugVar != null && debugVar == "1")
                config.LogLevel = LogSeverity.Debug;

            return config;
        }

        private DiscordSocketClient MakeClient(IServiceProvider services)
        {
            var config = services.GetRequiredService<DiscordSocketConfig>();
            var client = new DiscordSocketClient(config);

            client.Log += LogAsync;
            client.Ready += ReadyAsync;
            //client.InteractionCreated += InteractionCreated;

            return client;
        }

        private InteractionServiceConfig MakeInteractionConfig(IServiceProvider provider)
        {
            var config = new InteractionServiceConfig();
            config.AutoServiceScopes = true;
            config.EnableAutocompleteHandlers = true;

            return config;
        }

        private InteractionService MakeInteraction(IServiceProvider provider)
        {
            var client = provider.GetRequiredService<DiscordSocketClient>();
            var config = provider.GetRequiredService<InteractionServiceConfig>();

            var interaction = new InteractionService(client, config);
            interaction.AddTypeConverter<CurrencyModel>(new CurrencyModelConverter());

            return interaction;
        }

        private CommandHandlingService MakeCommandService(IServiceProvider provider)
        {
            var service = new CommandHandlingService(provider);
            return service;
        }

        private TimeService MakeTimeService(IServiceProvider provider)
        {
            var service = new TimeService();
            return service;
        }

        public async Task Start()
        {
            try
            {
                await currencyConverter.MakeCacheAsync();
                var token = Environment.GetEnvironmentVariable("BOT_TOKEN");

                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
            }

            await Task.Delay(Timeout.Infinite);
        }

        // private async Task InteractionCreated(SocketInteraction arg)
        // {
        //     if (arg != null)
        //     {
        //         var ctx = new SocketInteractionContext(client, arg);
        //         IInteractionContext? botContext;

        //         switch (arg.Type)
        //         {
        //             case InteractionType.ApplicationCommand:
        //                 botContext = new DarkBotContext<SocketSlashCommand>(client, (SocketSlashCommand)arg);
        //                 break;
        //             case InteractionType.MessageComponent:
        //                 botContext = new DarkBotContext<SocketMessageCommand>(client, (SocketMessageCommand)arg);
        //                 break;
        //             case InteractionType.ModalSubmit:
        //                 botContext = new DarkBotContext<SocketModal>(client, (SocketModal)arg);
        //                 break;
        //             case InteractionType.ApplicationCommandAutocomplete:
        //                 botContext = new DarkBotContext<SocketAutocompleteInteraction>(client, (SocketAutocompleteInteraction)arg);
        //                 break;
        //             default:
        //             case InteractionType.Ping:
        //                 botContext = new DarkBotContext<SocketUserCommand>(client, (SocketUserCommand)arg);
        //                 break;
        //         }

        //         await interaction.ExecuteCommandAsync(botContext, null);
        //     }
        // }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private async Task ReadyAsync()
        {
            Console.WriteLine($"{client.CurrentUser} is connected!");

            _timeService.ReadyDate = DateTime.Now;
            await commandHandling.InitializeAsync();

            await client.SetActivityAsync(new Game("Bot developed on C# (CSharp)", ActivityType.Playing));
            await client.SetStatusAsync(UserStatus.Online);

            await Task.CompletedTask;
        }
    }
}