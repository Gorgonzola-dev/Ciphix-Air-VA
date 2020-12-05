using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CiphixAir.API.Models;
using CiphixAir.Core.Helpers;
using CiphixAir.Core.Models;
using CiphixAir.Core.Models.OpenWeatherMap;
using CiphixAir.Core.Models.OpenWeatherMap.CurrentWeather;
using CiphixAir.Core.Models.OpenWeatherMap.Forecast;

namespace CiphixAir.Core.Services
{
    public class OpenWeatherMapService
    {
        HttpClient _client = new HttpClient();
        private string _key;
        public OpenWeatherMapService(string apiKey)
        {
            _key = apiKey;
            _client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");

        }

        public async Task<WeatherForecast> GetWeatherForecastForNow(WeatherRequest weatherRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"weather?q={weatherRequest.City}&appid={_key}");
            var response = await _client.SendAsync(request);
            var forecast = await ForecastBuilder.BuildCurrentWeatherForecast(response, weatherRequest);
            return forecast;
        }

        public async Task<WeatherForecast> GetWeatherForecastForPeriod(WeatherRequest weatherRequest)
        {
            var weatherNow = await GetWeatherForecastForNow(weatherRequest);
            var requestUri = GetRequestUriBasedOnRequestedTime(weatherRequest, weatherNow);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var response = await _client.SendAsync(request);
            var forecast = await ForecastBuilder.BuildFutureWeatherForecast(response, weatherNow);
            return forecast;
        }

        private string GetRequestUriBasedOnRequestedTime(WeatherRequest weatherRequest, WeatherForecast weatherNow)
        {
            if (weatherRequest.DateTime.Subtract(DateTime.UtcNow).TotalHours < 48)
            {
                return $"onecall?lat={weatherNow.Latitude}&lon={weatherNow.Longitude}&exclude=current,minutely,daily&appid={_key}";
            }

            return $"onecall?lat={weatherNow.Latitude}&lon={weatherNow.Longitude}&exclude=current,minutely,hourly&appid={_key}";
        }
    }
}
