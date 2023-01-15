using Discord;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tDarkBot.Models;
using tDarkBot.Services;

namespace tDarkBot.TypeConverters
{
    public class CurrencyModelConverter : TypeConverter
    {
        public override bool CanConvertTo(Type type)
        {
            return type == typeof(CurrencyModel);
        }

        public override ApplicationCommandOptionType GetDiscordType()
        {
            return ApplicationCommandOptionType.String;
        }

        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
        {
            try
            {
                if (option.Value == null)
                    return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, "Item parameter must be int native"));

                var converter = services.GetService(typeof(CurrencyConverterService)) as CurrencyConverterService;
                if (converter == null)
                    return Task.FromResult(TypeConverterResult.FromError(new InvalidOperationException("The converter service is not registered")));

                var code = (string)option.Value;
                var model = converter.GetCurrencyByCode(code);

                if (model == null)
                    return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.Unsuccessful, "Item ID not found into DB cache"));

                return Task.FromResult(TypeConverterResult.FromSuccess(model));
            }
            catch (Exception ex)
            {
                return Task.FromResult(TypeConverterResult.FromError(ex));
            }
        }
    }
}
