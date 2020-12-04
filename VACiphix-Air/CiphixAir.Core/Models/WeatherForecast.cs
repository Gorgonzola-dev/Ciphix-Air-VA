using System;
using System.Globalization;
using System.Linq;
using CiphixAir.Core.Models;
using CiphixAir.Core.Models.OpenWeatherMap;
using CiphixAir.Core.Models.OpenWeatherMap.CurrentWeather;
using CiphixAir.Core.Models.OpenWeatherMap.Forecast;

namespace CiphixAir.API.Models
{
    public struct WeatherForecast
    {
        public WeatherForecast(CurrentWeatherMapBase currentWeatherMapBase, WeatherRequest weatherRequest)
        {
            Date = weatherRequest.DateTime;
            TemperatureF = (int)((currentWeatherMapBase.CurrentWeatherMain.Temp - 273.15) * 9 / 5 + 32);
            TemperatureC = (int)(currentWeatherMapBase.CurrentWeatherMain.Temp - 273.15);
            Summary = currentWeatherMapBase.Weather.FirstOrDefault()?.Description;
            City = currentWeatherMapBase.Name;
            Latitude = currentWeatherMapBase.CurrentWeatherCoordinates.Lat.ToString(CultureInfo.InvariantCulture);
            Longitude = currentWeatherMapBase.CurrentWeatherCoordinates.Lon.ToString(CultureInfo.InvariantCulture);
            IsForecast = false;
        }

        public WeatherForecast(ForecastWeatherMapBase forecastWeather, WeatherForecast weatherRequest)
        {
            var time = forecastWeather.hourly.Select(forecast => forecast.DateTime).OrderBy(t => Math.Abs((t - weatherRequest.Date).Ticks)).First();
            var foreCast = forecastWeather.hourly.First(forecast => forecast.DateTime == time);

            Date = foreCast.DateTime;
            TemperatureF = (int)((foreCast.temp - 273.15) * 9 / 5 + 32);
            TemperatureC = (int)(foreCast.temp - 273.15);
            Summary = foreCast.weather.FirstOrDefault()?.description;
            City = weatherRequest.City;
            Latitude = weatherRequest.Latitude;
            Longitude = weatherRequest.Longitude;
            IsForecast = true;
        }

        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF { get; set; }
        public string Summary { get; set; }
        public string City { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool IsForecast { get; set; }
    }
}
