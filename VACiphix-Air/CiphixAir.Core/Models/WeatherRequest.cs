using System;
using System.Text.Json.Serialization;
using CiphixAir.Core.Models.Converters;

namespace CiphixAir.Core.Models
{
    public class WeatherRequest
    {
        [JsonPropertyName("geo-city")]
        public string City { get; set; }

        [JsonConverter(typeof(StringToDateTimeConverter))]
        [JsonPropertyName("date-time")]
        public DateTime DateTime { get; set; } = System.DateTime.Now;       
        
        [JsonPropertyName("time-period")]
        public string TimePeriod { get; set; }

        [JsonPropertyName("forFlight")]
        public string ForFlight { get; set; }

        [JsonConverter(typeof(StringToBoolConverterIfStringIsDateTime))]
        [JsonPropertyName("DateTimeHasValue")]
        public bool DateTimeGiven { get; set; }
    }
}