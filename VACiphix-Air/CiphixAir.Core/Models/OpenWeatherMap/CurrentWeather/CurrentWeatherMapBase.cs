using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CiphixAir.Core.Models.OpenWeatherMap.CurrentWeather
{
    public class CurrentWeatherMapBase
    {
        [JsonPropertyName("coord")]
        public CurrentWeatherCoordinates CurrentWeatherCoordinates { get; set; }

        [JsonPropertyName("weather")]
        public List<CurrentWeather> Weather { get; set; }

        [JsonPropertyName("base")]
        public string BasedOn { get; set; }

        [JsonPropertyName("main")]
        public CurrentWeatherMain CurrentWeatherMain { get; set; }

        [JsonPropertyName("visibility")]
        public int Visibility { get; set; }

        [JsonPropertyName("wind")]
        public CurrentWind CurrentWind { get; set; }
        
        [JsonPropertyName("clouds")]
        public CurrentClouds CurrentClouds { get; set; }

        [JsonPropertyName("dt")]
        public int DateTimeUtc { get; set; }

        [JsonPropertyName("sys")]
        public CurrentWeatherData CurrentWeatherData { get; set; }

        [JsonPropertyName("timezone")]
        public int Timezone { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("cod")]
        public int Cod { get; set; }
    }
}
