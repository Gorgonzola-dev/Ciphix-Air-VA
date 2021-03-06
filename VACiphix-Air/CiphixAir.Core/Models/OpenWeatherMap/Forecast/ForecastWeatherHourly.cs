using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CiphixAir.Core.Models.OpenWeatherMap.Forecast
{
    public class ForecastWeatherHourly
    {
        [JsonConverter(typeof(IntToDateTimeConverter))]
        [JsonPropertyName("dt")]
        public DateTime DateTime { get; set; }
        [JsonPropertyName("temp")]
        public double Temperature { get; set; }
        public double feels_like { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public double dew_point { get; set; }
        public double uvi { get; set; }
        public int clouds { get; set; }
        public int visibility { get; set; }
        public double wind_speed { get; set; }
        public int wind_deg { get; set; }
        [JsonPropertyName("weather")]
        public List<ForecastWeather> Weathers { get; set; }
        public double pop { get; set; }
    }
}