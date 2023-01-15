using Discord;
using Discord.Interactions;
using tDarkBot.Services;
using System.Linq;

namespace tDarkBot.AutoCompleteHandlers
{
    public class CurrencyAutoComplete : AutocompleteHandler
    {
        public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction interaction, IParameterInfo parameter, IServiceProvider services)
        {
            var filter = interaction?.Data?.Current?.Value?.ToString() ?? "";
            var converter = services.GetService(typeof(CurrencyConverterService)) as CurrencyConverterService;

            if (converter == null)
                return Task.FromResult(AutocompletionResult.FromError(new InvalidOperationException("The service provider has not converter registered!")));

            var currencies = converter.FilterCurrencies(filter, 25);
            var results = currencies.Select(c => new AutocompleteResult($"{c.Code} - {c.Name}", c.Code));

            return Task.FromResult(AutocompletionResult.FromSuccess(results));
        }
    }
}