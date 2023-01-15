using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using tDarkBot.Services;

namespace tDarkBot
{
    public class DarkBot
    {
        private DiscordSocketClient client;
        private InteractionService interaction;
        private CurrencyConverterService currencyConverter;

        public DarkBot()
        {
            var config = new DiscordSocketConfig();
            config.GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.MessageContent | GatewayIntents.Guilds | GatewayIntents.GuildMessages;
            config.UseInteractionSnowflakeDate = true;
            config.UseSystemClock = true;
            config.AlwaysDownloadUsers = true;


            client = new DiscordSocketClient(config);
            client.Log += LogAsync;
            client.Ready += ReadyAsync;
            client.InteractionCreated += InteractionCreated;

            interaction = new InteractionService(client);
            interaction.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            currencyConverter = new CurrencyConverterService();
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

        private async Task InteractionCreated(SocketInteraction arg)
        {
            if (arg != null)
            {
                var ctx = new SocketInteractionContext(client, arg);
                IInteractionContext? botContext;

                switch (arg.Type)
                {
                    case InteractionType.ApplicationCommand:
                        botContext = new DarkBotContext<SocketSlashCommand>(client, arg as SocketSlashCommand);
                        break;
                    case InteractionType.MessageComponent:
                        botContext = new DarkBotContext<SocketMessageCommand>(client, arg as SocketMessageCommand);
                        break;
                    case InteractionType.ModalSubmit:
                        botContext = new DarkBotContext<SocketModal>(client, arg as SocketModal);
                        break;
                    default:
                    case InteractionType.Ping:
                        botContext = new DarkBotContext<SocketUserCommand>(client, arg as SocketUserCommand);
                        break;
                }

                await interaction.ExecuteCommandAsync(botContext, null);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"{client.CurrentUser} is connected!");

            interaction.RegisterCommandsGloballyAsync();

            client.SetActivityAsync(new Game("Bot developed on C# (CSharp)", ActivityType.Playing));
            client.SetStatusAsync(UserStatus.Online);

            return Task.CompletedTask;
        }
    }
}