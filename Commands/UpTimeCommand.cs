using System.Text;
using Discord;
using Discord.Interactions;
using tDarkBot.Services;
using Humanizer;

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
            var readyDate = _timeService.ReadyDate.ToString("yyyy-MM-dd HH.mm.ss");
            var onlineSince = (DateTime.Now - _timeService.CreationDate).Humanize();

            var contentBuilder = new StringBuilder();
            contentBuilder.AppendLine($"Create Time: {createDate}");
            contentBuilder.AppendLine($"UpTime: {readyDate}");
            contentBuilder.AppendLine($"Online since: {onlineSince}");

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