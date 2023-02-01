using System.Text;
using Discord;
using Discord.Interactions;
using Newtonsoft.Json;
using tDarkBot.Models;

namespace tDarkBot.Commands
{
    public class ProfileImportCommand : BaseCommand
    {
        [SlashCommand("profile-import", "Generate the JSON file for user template")]
        public async Task Command()
        {
            await Context.Interaction.DeferAsync();

            var user = Context.Interaction.User;
            var model = new ProfileModel(user.Username);

            if (Context.Interaction.IsDMInteraction)
            {
                var restUser = await Context.Client.Rest.GetUserAsync(user.Id);
                model.AvatarURL = restUser.GetAvatarUrl(size: 4096) ?? restUser.GetDefaultAvatarUrl();
                model.BannerURL = restUser.GetBannerUrl(size: 4096);
            }
            else
            {
                var guildID = Context.Guild.Id;
                var restUser = await Context.Client.Rest.GetGuildUserAsync(Context.Guild.Id, user.Id);

                model.AvatarURL = restUser.GetAvatarUrl(size: 4096) ?? restUser.GetDefaultAvatarUrl();
                model.BannerURL = restUser.GetBannerUrl(size: 4096);
            }

            var jsonContent = JsonConvert.SerializeObject(model, Formatting.Indented);
            var stringB = new StringBuilder();
            stringB.AppendLine("```json");
            stringB.AppendLine(jsonContent);
            stringB.AppendLine("```");

            var builder = new EmbedBuilder()
            .WithAuthor(Context.Client.CurrentUser)
            .WithCurrentTimestamp()
            .WithDescription(stringB.ToString());

            var embed = builder.Build();

            await Context.Interaction.FollowupAsync("Data has been generated!", embed: embed);
        }
    }
}