using System.Text.Json.Serialization;

namespace CiphixAir.Core.Models.OpenWeatherMap.CurrentWeather
{
    public class CurrentClouds
    {
        [JsonPropertyName("all")]
        public int All { get; set; }
    }
}