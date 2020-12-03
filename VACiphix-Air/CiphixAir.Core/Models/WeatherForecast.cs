using System;
using System.Linq;
using CiphixAir.Core.Models.OpenWeatherMap;

namespace CiphixAir.API.Models
{
    public struct WeatherForecast
    {
        public WeatherForecast(OpenWeatherMapBase openWeatherMapBase)
        {
            Date = DateTime.MinValue.AddSeconds(openWeatherMapBase.DateTimeUtc).AddSeconds(openWeatherMapBase.Timezone); //TODO: Fix this hot mess
            TemperatureF = (int)openWeatherMapBase.Main.Temp;
            Summary = openWeatherMapBase.Weather.FirstOrDefault()?.Description;
        }

        public DateTime Date { get; set; }

        public int TemperatureC => (TemperatureF - 32) * 5 / 9; //OpenWeather gives back Fahrenheit, converted here to Celsius

        public int TemperatureF { get; set; }

        public string Summary { get; set; }
    }
}
