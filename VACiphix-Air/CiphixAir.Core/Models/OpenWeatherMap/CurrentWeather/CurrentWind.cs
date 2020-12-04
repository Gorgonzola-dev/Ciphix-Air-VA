using System.Text.Json.Serialization;

namespace CiphixAir.Core.Models.OpenWeatherMap.CurrentWeather
{
    public class CurrentWind
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }

        [JsonPropertyName("deg")]
        public int Deg { get; set; }
    }
}