using Discord;
using Discord.Interactions;

namespace tDarkBot.Commands
{
    public enum BannerSize : ushort
    {
        Tiny = 16,
        Smaller = 23,
        Smallest = 64,
        Small = 128,
        Medium = 256,
        Large = 512,
        Largest = 2048,
        Maximum = 4096
    }

    public class BannerCommand : BaseCommand
    {
        private HttpClient _client;
        public BannerCommand(HttpClient client)
        {
            _client = client;
        }

        private async Task<Stream> FetchImage(string url)
        {
            return await _client.GetStreamAsync(url);
        }

        [SlashCommand("banner", "Retrieve sender (or user in param) banner")]
        public async Task Command
        (
            [Summary("user", "The user that fetchs the banner")] IUser? user = null,
            [Summary("size", "The banner min size")] BannerSize size = BannerSize.Maximum,
            [Summary("format", "The avatar image format")] ImageFormat format = ImageFormat.Auto,
            [Summary("cache", "Determine if the bot downloads the file and send as attachment")] bool cache = false
        )
        {
            await Context.Interaction.DeferAsync();

            if (user == null)
                user = Context.Interaction.User;

            string? url = null;
            if (Context.Interaction.IsDMInteraction)
            {
                var restUser = await Context.Client.Rest.GetUserAsync(user.Id);
                url = restUser.GetBannerUrl(format, (ushort)size);
            }
            else
            {
                var guildID = Context.Guild.Id;
                var restUser = await Context.Client.Rest.GetGuildUserAsync(Context.Guild.Id, user.Id);

                url = restUser.GetBannerUrl(format, (ushort)size);
            }

            var builder = new EmbedBuilder()
            .WithAuthor(Context.Client.CurrentUser)
            .WithCurrentTimestamp();
            //.Build();

            string? filename = null;
            Stream? stream = null;

            if (url == null)
            {
                builder = builder
                .WithColor(Color.Red)
                .WithDescription("Failed to fetch the user banner, or user hasn't a banner!");
            }
            else
            {
                builder = builder
                .WithColor(Color.Blue)
                .WithDescription("Banner has been fetch, caching the image...");

                if (cache)
                {
                    stream = await FetchImage(url);
                    filename = Path.GetFileName(url);
                    if (filename.IndexOf('?') > -1)
                        filename = filename.Substring(0, filename.IndexOf('?'));
                }
                else
                {
                    builder = builder.WithImageUrl(url);
                }
            }

            var embed = builder.Build();

            if (url == null)
                await Context.Interaction.FollowupAsync("Failed to fetch the banner!", embed: embed);
            else if (stream == null)
                if (cache)
                    await Context.Interaction.FollowupAsync("Failed to fetch the image from URL: " + (url ?? "NULL URL"));
                else
                    await Context.Interaction.FollowupAsync(embed: embed);
            else
                await Context.Interaction.FollowupWithFileAsync(stream, filename, embed: embed);
        }
    }
}