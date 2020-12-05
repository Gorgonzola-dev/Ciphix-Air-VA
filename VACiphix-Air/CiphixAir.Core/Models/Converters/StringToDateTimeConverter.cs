using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CiphixAir.Core.Models.Converters
{
    public class StringToDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (var jsonDoc = JsonDocument.ParseValue(ref reader))
            {
                var dateTime = DateTime.UtcNow;

                if (jsonDoc.RootElement.ValueKind == JsonValueKind.String)
                {
                    if (string.IsNullOrWhiteSpace(jsonDoc.RootElement.GetString()))
                    {
                        return dateTime;
                    }
                    dateTime = DateTime.Parse(jsonDoc.RootElement.ToString());
                    return dateTime;
                }
                if (jsonDoc.RootElement.TryGetProperty("date_time", out var dateTimeObject))
                {
                    if (dateTimeObject.TryGetDateTime(out dateTime))
                    {
                        return dateTime;
                    }
                }


                return dateTime;
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-ddThh:mm:ss"));
        }
    }
}