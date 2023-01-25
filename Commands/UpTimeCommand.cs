using System.Text;
using Discord;
using Discord.Interactions;
using tDarkBot.Services;

namespace tDarkBot.Commands
{
    public class UpTimeCommand : BaseCommand
    {
        private TimeService _timeService;
        public UpTimeCommand(TimeService timeService)
        {
            _timeService = timeService;
        }

        [SlashCommand("uptime", "Shows the bot uptime dates")]
        async Task Command()
        {
            var createDate = _timeService.CreationDate.ToString("yyyy-MM-dd HH.mm.ss");
            var readydate = _timeService.ReadyDate.ToString("yyyy-MM-dd HH.mm.ss");

            var contentBuilder = new StringBuilder();
            contentBuilder.AppendLine($"Create Time: {createDate}");
            contentBuilder.AppendLine($"UpTime: {readydate}");

            var embed = new EmbedBuilder()
            .WithAuthor(Context.Client.CurrentUser)
            .WithTitle("Uptime Info")
            .WithColor(Color.DarkBlue)
            .WithCurrentTimestamp()
            .WithDescription(contentBuilder.ToString())
            .Build();

            await Context.Interaction.RespondAsync(embed: embed);
            await Task.CompletedTask;
        }
    }
}