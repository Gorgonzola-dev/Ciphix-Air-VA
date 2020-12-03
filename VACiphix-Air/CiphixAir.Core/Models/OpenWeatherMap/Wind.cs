using System.Text.Json.Serialization;

namespace CiphixAir.Core.Models.OpenWeatherMap
{
    public class Wind
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }

        [JsonPropertyName("deg")]
        public int Deg { get; set; }
    }
}