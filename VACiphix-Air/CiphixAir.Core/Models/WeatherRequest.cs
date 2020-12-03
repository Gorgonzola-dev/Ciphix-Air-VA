using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CiphixAir.Core.Models
{
    public class WeatherRequest
    {
        [JsonPropertyName("geo-city")]
        public string City { get; set; }

        [JsonConverter(typeof(StringToDateTimeConverter))]
        [JsonPropertyName("date-time")]
        public DateTime DateTime { get; set; } = System.DateTime.Now;
        
        [JsonPropertyName("forFlight")] 
        public string ForFlight { get; set; }
    }

    public class StringToDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (var jsonDoc = JsonDocument.ParseValue(ref reader))
            {
                var dateTime = DateTime.UtcNow;
                if (jsonDoc.RootElement.TryGetProperty("date_time", out var dateTimeElement))
                {
                    dateTime = DateTime.Parse(dateTimeElement.ToString());
                }
                else
                {
                    if (!jsonDoc.RootElement.TryGetDateTime(out dateTime))
                    {
                        dateTime = DateTime.Parse(jsonDoc.RootElement.ToString());
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