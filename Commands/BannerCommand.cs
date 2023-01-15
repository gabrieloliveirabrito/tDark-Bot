// using Discord;
// using Discord.Interactions;

// namespace tDarkBot.Commands
// {
//     public enum BannerSize : ushort
//     {
//         Tiny = 16,
//         Smaller = 23,
//         Smallest = 64,
//         Small = 128,
//         Medium = 256,
//         Large = 512,
//         Largest = 2048,
//         Maximum = 4096
//     }

//     public class BannerCommand : BaseCommand
//     {
//         [SlashCommand("banner", "Retrieve sender (or user in param) banner")]
//         public async Task Command
//         (
//             [Summary("user", "The user that fetchs the banner")] IUser? user = null,
//             [Summary("size", "The banner min size")] BannerSize size = BannerSize.Small
//         )
//         {
//             if (user == null)
//                 user = Context.Interaction.User;

//             var url = Context.Interaction.User.Id;

//             var embed = new EmbedBuilder()
//             .WithColor(Color.Blue)
//             .WithImageUrl(url)
//             .WithAuthor(Context.Client.CurrentUser)
//             .WithCurrentTimestamp()
//             .Build();

//             await Context.Interaction.RespondAsync("Avatar retrieved", embed: embed);
//         }
//     }
// }