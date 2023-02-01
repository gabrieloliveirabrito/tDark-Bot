using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;

namespace tDarkBot.Commands
{
    public enum AvatarSize : ushort
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

    public class AvatarCommand : BaseCommand
    {
        private HttpClient _client;
        public AvatarCommand(HttpClient client)
        {
            _client = client;
        }

        private async Task<Stream> FetchImage(string url)
        {
            return await _client.GetStreamAsync(url);
        }

        [SlashCommand("avatar", "Retrieve sender (or user in param) avatar")]
        public async Task Command
        (
            [Summary("user", "The user that fetchs the avatar")] IUser? user = null,
            [Summary("size", "The avatar min size")] AvatarSize size = AvatarSize.Medium,
            [Summary("format", "The avatar image format")] ImageFormat format = ImageFormat.Auto
        )
        {
            if (user == null)
                user = Context.Interaction.User;

            await Context.Interaction.DeferAsync();

            var url = user.GetAvatarUrl(format, (ushort)size) ?? user.GetDefaultAvatarUrl();

            var stream = await FetchImage(url);
            var filename = Path.GetFileName(url);
            if (filename.IndexOf('?') > -1)
                filename = filename.Substring(0, filename.IndexOf('?'));

            await Context.Interaction.FollowupWithFileAsync(stream, filename, "Avatar fetch and cached");
        }
    }
}