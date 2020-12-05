using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CiphixAir.Core.Models.OpenWeatherMap.Forecast
{
    public class ForecastWeatherDaily
    {
        [JsonConverter(typeof(IntToDateTimeConverter))]
        [JsonPropertyName("dt")]
        public DateTime DateTime { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
        [JsonPropertyName("temp")]
        public DailyTemperatures DailyTemperatures { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public double dew_point { get; set; }
        public double wind_speed { get; set; }
        public int wind_deg { get; set; }
        [JsonPropertyName("weather")]
        public List<ForecastWeather> Weathers { get; set; }
        public decimal clouds { get; set; }
        [JsonPropertyName("pop")]
        public decimal ChanceOfPrecipitation {get; set; }
        public double rain { get; set; }
        public double uvi { get; set; }
    }
}