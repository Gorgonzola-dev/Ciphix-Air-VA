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
        
        [JsonPropertyName("flightNumber")]
        public int ForFlight { get; set; }

        [JsonPropertyName("airline")]
        public string FlightProvider { get; set; }

        public IntentType Intent { get; set; }

        public WeatherRequest PreviousWeatherRequest { get; set; }
    }
}