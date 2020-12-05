using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CiphixAir.API.Models;
using CiphixAir.Core.Models;
using CiphixAir.Core.Models.OpenWeatherMap.CurrentWeather;
using CiphixAir.Core.Models.OpenWeatherMap.Forecast;

namespace CiphixAir.Core.Helpers
{
    public static class ForecastBuilder
    {
        //BuildCurrent and BuildFuture need to be separate methods because the model that the OpenWeatherMap returns are so different that they can't be mapped to the same object easily
        public static async Task<WeatherForecast> BuildFutureWeatherForecast(HttpResponseMessage response, WeatherForecast weatherNow)
        {
            var obj = new ForecastWeatherMapBase();
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                obj = await JsonSerializer.DeserializeAsync<ForecastWeatherMapBase>(stream);
            }

            var weatherForecast = BuildForecastFromWeatherMap(obj, weatherNow);

            return weatherForecast;
        }

        public static async Task<WeatherForecast> BuildCurrentWeatherForecast(HttpResponseMessage response,
            WeatherRequest weatherRequest)
        {
            var obj = new CurrentWeatherMapBase();
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                obj = await JsonSerializer.DeserializeAsync<CurrentWeatherMapBase>(stream);
            }
            return new WeatherForecast(obj, weatherRequest);
        }

        private static WeatherForecast BuildForecastFromWeatherMap(ForecastWeatherMapBase weatherMapBase,
            WeatherForecast nowForecast)
        {

            if (weatherMapBase.hourly != null)
            {
                return BuildForecastFromHourlyWeather(weatherMapBase, nowForecast);
            }

            return BuildForecastFromDailyWeather(weatherMapBase, nowForecast);
        }

        private static WeatherForecast BuildForecastFromDailyWeather(ForecastWeatherMapBase weatherMapBase,
            WeatherForecast nowForecast)
        {
            var weatherForecast = new WeatherForecast();

            var time = weatherMapBase.daily.Select(forecast => forecast.DateTime).OrderBy(t => Math.Abs((t - nowForecast.DateTime).Ticks)).First();
            var foreCast = weatherMapBase.daily.First(forecast => forecast.DateTime == time);

            weatherForecast.DateTime = foreCast.DateTime;
            weatherForecast.TemperatureDayInFahrenheit = (int)((foreCast.DailyTemperatures.day - 273.15) * 9 / 5 + 32);
            weatherForecast.TemperatureNightInFahrenheit= (int)((foreCast.DailyTemperatures.night - 273.15) * 9 / 5 + 32);
            weatherForecast.TemperatureDayInCelsius = (int)(foreCast.DailyTemperatures.day - 273.15);
            weatherForecast.TemperatureNightInCelsius = (int)(foreCast.DailyTemperatures.night - 273.15);
            weatherForecast.Summary = foreCast.Weathers.FirstOrDefault()?.description;
            weatherForecast.City = nowForecast.City;
            weatherForecast.Latitude = nowForecast.Latitude;
            weatherForecast.Longitude = nowForecast.Longitude;
            weatherForecast.IsForecast = true;
            return weatherForecast;
        }

        private static WeatherForecast BuildForecastFromHourlyWeather(ForecastWeatherMapBase weatherMapBase,
            WeatherForecast nowForecast)
        {
            var weatherForecast = new WeatherForecast();

            var time = weatherMapBase.hourly.Select(forecast => forecast.DateTime).OrderBy(t => Math.Abs((t - nowForecast.DateTime).Ticks)).First();
            var foreCast = weatherMapBase.hourly.First(forecast => forecast.DateTime == time);
            
            weatherForecast.DateTime = foreCast.DateTime;
            weatherForecast.TemperatureDayInFahrenheit = (int)((foreCast.Temperature - 273.15) * 9 / 5 + 32);
            weatherForecast.TemperatureDayInCelsius = (int)(foreCast.Temperature - 273.15);
            weatherForecast.Summary = foreCast.Weathers.FirstOrDefault()?.description;
            weatherForecast.City = nowForecast.City;
            weatherForecast.Latitude = nowForecast.Latitude;
            weatherForecast.Longitude = nowForecast.Longitude;
            weatherForecast.IsForecast = true;
            return weatherForecast;
        }
    }
}