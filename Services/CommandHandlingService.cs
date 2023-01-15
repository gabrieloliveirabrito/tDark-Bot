
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace tDarkBot.Services
{
    public class CommandHandlingService
    {
        private readonly InteractionService _interaction;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        public CommandHandlingService(IServiceProvider services)
        {
            _interaction = services.GetRequiredService<InteractionService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
        }

        public async Task InitializeAsync()
        {
            await _interaction.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            await _interaction.RegisterCommandsGloballyAsync(true);
            _discord.InteractionCreated += HandleInteraction;
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            try
            {
                // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
                var context = new SocketInteractionContext(_discord, interaction);

                // Execute the incoming command.
                var result = await _interaction.ExecuteCommandAsync(context, _services);

                if (!result.IsSuccess)
                    switch (result.Error)
                    {
                        case InteractionCommandError.UnmetPrecondition:
                            await interaction.RespondAsync("UnmetPrecondition");
                            break;
                        default:
                            await interaction.RespondAsync(result.Error.ToString());
                            break;
                    }
            }
            catch (Exception ex)
            {
                await interaction.RespondAsync(ex.Message);
                // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (interaction.Type is InteractionType.ApplicationCommand)
                    await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}