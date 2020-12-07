using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CiphixAir.Core.Models.Converters
{
    //Unused, using different Intents allowed us to not need this, keeping it here for now to show what i did before
    public class StringToBoolConverterIfStringIsDateTime : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (var jsonDoc = JsonDocument.ParseValue(ref reader))
            {
                var type = jsonDoc.RootElement.ValueKind;
                if (jsonDoc.RootElement.ValueKind == JsonValueKind.String)
                {
                    if (string.IsNullOrWhiteSpace(jsonDoc.RootElement.GetString()))
                    {
                        return false;
                    }

                    return jsonDoc.RootElement.TryGetDateTime(out _);
                }
                if (jsonDoc.RootElement.TryGetProperty("date_time", out var dateTimeObject))
                {
                    if (dateTimeObject.TryGetDateTime(out _))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}