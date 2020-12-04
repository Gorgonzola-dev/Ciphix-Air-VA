using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CiphixAir.Core.Models
{
    public class IntToDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (var jsonDoc = JsonDocument.ParseValue(ref reader))
            {
                var dateTime = new DateTime(1970,1,1);
                dateTime = dateTime.AddSeconds(jsonDoc.RootElement.GetInt32());
                return dateTime;
            }
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-ddThh:mm:ss"));
        }
    }
}