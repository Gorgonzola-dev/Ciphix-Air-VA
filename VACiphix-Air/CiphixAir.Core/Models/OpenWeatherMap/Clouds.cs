using System.Text.Json.Serialization;

namespace CiphixAir.Core.Models.OpenWeatherMap
{
    public class Clouds
    {
        [JsonPropertyName("all")]
        public int All { get; set; }
    }
}