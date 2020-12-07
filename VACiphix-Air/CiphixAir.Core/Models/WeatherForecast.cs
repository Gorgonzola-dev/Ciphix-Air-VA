using System;
using System.Globalization;
using System.Linq;
using CiphixAir.Core.Models;
using CiphixAir.Core.Models.OpenWeatherMap.CurrentWeather;

namespace CiphixAir.API.Models
{
    public class WeatherForecast
    {
        public WeatherForecast()
        {
        }
        public WeatherForecast(CurrentWeatherMapBase currentWeatherMapBase, WeatherRequest weatherRequest)
        {
            DateTime = weatherRequest.DateTime;
            TemperatureDayInFahrenheit = (int)((currentWeatherMapBase.CurrentWeatherMain.Temperature - 273.15) * 9 / 5 + 32);
            TemperatureDayInCelsius = (int)(currentWeatherMapBase.CurrentWeatherMain.Temperature - 273.15);
            Summary = currentWeatherMapBase.Weather.FirstOrDefault()?.Description;
            City = currentWeatherMapBase.Name;
            Latitude = currentWeatherMapBase.CurrentWeatherCoordinates.Lat.ToString(CultureInfo.InvariantCulture);
            Longitude = currentWeatherMapBase.CurrentWeatherCoordinates.Lon.ToString(CultureInfo.InvariantCulture);
        }


        public DateTime DateTime { get; set; }
        public int TemperatureDayInCelsius { get; set; }
        public int? TemperatureNightInCelsius { get; set; }
        public int TemperatureDayInFahrenheit { get; set; }
        public int? TemperatureNightInFahrenheit { get; set; }
        public string Summary { get; set; }
        public string City { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool IsForecast { get; set; }
    }
}
