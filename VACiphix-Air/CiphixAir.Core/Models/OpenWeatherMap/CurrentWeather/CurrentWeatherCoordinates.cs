using System.Text.Json.Serialization;

namespace CiphixAir.Core.Models.OpenWeatherMap.CurrentWeather
{
    public class CurrentWeatherCoordinates
    {
        [JsonPropertyName("lon")]
        public double Lon { get; set; }

        [JsonPropertyName("lat")]
        public double Lat { get; set; }
    }
}