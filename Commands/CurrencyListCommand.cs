using Discord;
using Discord.Interactions;
using tDarkBot.AutoCompleteHandlers;
using tDarkBot.Models;
using tDarkBot.Services;

namespace tDarkBot.Commands
{
    public class CurrencyListCommand : BaseCommand
    {
        private CurrencyConverterService _service;

        public CurrencyListCommand(CurrencyConverterService service)
        {
            _service = service;
        }

        [SlashCommand("currency-list", "Shows the available currencies to for conversion")]
        public async Task Command()
        {
            await Context.Interaction.DeferAsync();
            var currencies = _service.GetCachedCurrencies();

            var content = "The availabile currencies is:" + Environment.NewLine;
            content += string.Join(Environment.NewLine, currencies.Select(c => $"{c.Code} - {c.Name} - {c.Symbol}"));

            var embed = new EmbedBuilder()
            .WithColor(Color.DarkGreen)
            .WithTitle("Currency Converter")
            .WithDescription(content)
            .WithAuthor(Context.Client.CurrentUser)
            .WithCurrentTimestamp()
            .Build();

            await Context.Interaction.FollowupAsync(embed: embed);
            await Task.CompletedTask;
        }
    }
}