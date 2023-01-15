using Discord;
using Discord.Interactions;
using tDarkBot.AutoCompleteHandlers;
using tDarkBot.Models;
using tDarkBot.Services;

namespace tDarkBot.Commands
{
    public class CurrencyConverterCommand : BaseCommand
    {
        private CurrencyConverterService _service;

        public CurrencyConverterCommand(CurrencyConverterService service)
        {
            _service = service;
        }

        [SlashCommand("convert-currency", "Converts an value to currency")]
        public async Task Command
        (
            [Summary("from", "The currency that values has from"), Autocomplete(typeof(CurrencyAutoComplete))] CurrencyModel fromCurrency,
            [Summary("to", "The curency that vlaue has to be converted"), Autocomplete(typeof(CurrencyAutoComplete))] CurrencyModel toCurrency,
            [Summary("value", "The avatar image format")] decimal value
        )
        {
            await Context.Interaction.DeferAsync();

            if (fromCurrency.Code == null)
            {
                await Context.Interaction.RespondAsync("The from currency dont loaded successfully on bot!");
                return;
            }

            if (toCurrency.Code == null)
            {
                await Context.Interaction.RespondAsync("The to currency dont loaded successfully on bot!");
                return;
            }

            var result = await _service.ConvertCurrency(fromCurrency.Code, toCurrency.Code, value);
            var rounded = decimal.Round(result, 2, MidpointRounding.AwayFromZero);

            await Context.Interaction.FollowupAsync($"The converted value is {toCurrency.Symbol} {rounded}");
        }
    }
}