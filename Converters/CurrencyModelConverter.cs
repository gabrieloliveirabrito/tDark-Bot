using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using tDarkBot.Models;

namespace tDarkBot.Converters
{
    public class CurrencyModelConverter : JsonConverter<CurrencyModel>
    {
        public override CurrencyModel? ReadJson(JsonReader reader, Type objectType, CurrencyModel? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var model = new CurrencyModel();
            model.Symbol = (string?)token["symbol"];
            model.Name = (string?)token["name"];
            model.SymbolNative = (string?)token["symbol_native"];
            model.DecimalDigits = (int?)token["decimal_digits"];
            model.Rounding = (int?)token["rounding"];
            model.Code = (string?)token["code"];
            model.NamePlural = (string?)token["name_plural"];

            return model;
        }

        public override void WriteJson(JsonWriter writer, CurrencyModel? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteStartObject();
                writer.WritePropertyName("symbol");
                writer.WriteValue(value.Symbol);

                writer.WritePropertyName("name");
                writer.WriteValue(value.Name);

                writer.WritePropertyName("symbol_native");
                writer.WriteValue(value.SymbolNative);

                writer.WritePropertyName("decimal_digits");
                writer.WriteValue(value.DecimalDigits);

                writer.WritePropertyName("rounding");
                writer.WriteValue(value.Rounding);

                writer.WritePropertyName("code");
                writer.WriteValue(value.Code);

                writer.WritePropertyName("name_plural");
                writer.WriteValue(value.NamePlural);

                writer.WriteEndObject();
            }
        }
    }
}