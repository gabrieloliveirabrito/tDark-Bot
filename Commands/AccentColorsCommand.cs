using Discord;
using Discord.Interactions;

namespace tDarkBot.Commands
{
    public class AccentColorsCommand : BaseCommand
    {

        [SlashCommand("accents", "Retrieve sender (or user in param) accent colors of profile")]
        public async Task Command
        (
            [Summary("user", "The user that fetchs the accents color")] IUser? user = null
        )
        {
            await Context.Interaction.DeferAsync();

            if (user == null)
                user = Context.Interaction.User;

            Color? accentColor = null;
            if (Context.Interaction.IsDMInteraction)
            {
                var restUser = await Context.Client.Rest.GetUserAsync(user.Id);
                accentColor = restUser.AccentColor;
            }
            else
            {
                var guildID = Context.Guild.Id;
                var restUser = await Context.Client.Rest.GetGuildUserAsync(Context.Guild.Id, user.Id);

                accentColor = restUser.AccentColor;
            }

            var builder = new EmbedBuilder()
            .WithAuthor(Context.Client.CurrentUser)
            .WithCurrentTimestamp()
            .WithColor(accentColor ?? Color.Red);

            if (accentColor == null)
            {
                builder = builder
                .WithDescription("Failed to fetch the profile accent color, or user hasn't have nitro!");
            }
            else
            {
                builder = builder
                .WithDescription("0x" + accentColor.Value.RawValue.ToString("X"));
            }

            var embed = builder.Build();
            await Context.Interaction.FollowupAsync(embed: embed);
        }
    }
}