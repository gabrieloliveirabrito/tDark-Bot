using Discord;
using Discord.Interactions;

namespace tDarkBot.Commands
{
    public class CallUsersCommand : BaseCommand
    {
        [SlashCommand("call-users", "Call all users in category")]
        public async Task Execute
        (
           [Summary("role", "The role that fetch the users to mention")] IRole role,
           [Summary("message", "The message after call")] string message
        )
        {
            if (Context.Interaction.IsDMInteraction)
            {
                await Context.Interaction.RespondAsync("This command can only be send from guild!");
            }
            else if (!role.IsMentionable)
            {
                await Context.Interaction.RespondAsync("This command can only be used for mentionable roles!");
            }
            else if (role.Permissions.Administrator || role.Permissions.ModerateMembers)
            {
                await Context.Interaction.RespondAsync("This command can only be used for non moderation roles!");
            }
            else
            {
                string content = role.Mention + " ";
                var guildUsers = await role.Guild.GetUsersAsync();
                var mentions = new List<string>();
                foreach (var user in guildUsers)
                {
                    if (user.RoleIds.Contains(role.Id))
                        mentions.Add(user.Mention);
                }
                content += string.Join(" ", mentions);
                content += " - " + message;
                content += " - Mensagem enviada por " + Context.Interaction.User.Mention;

                await Context.Interaction.RespondAsync(content);
            }
            await Task.CompletedTask;
        }
    }
}