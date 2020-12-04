using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CiphixAir.API.Models;
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
        public async Task<WeatherForecast> GetWeatherForecast(WeatherRequest weatherRequest)
        {
            if (weatherRequest.DateTimeGiven)
            {
                return await GetWeatherForecastForPeriod(weatherRequest);
            }

            return await GetWeatherForecastForNow(weatherRequest);
        }

        private async Task<WeatherForecast> GetWeatherForecastForNow(WeatherRequest weatherRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"weather?q={weatherRequest.City}&appid={_key}");
            var response = await _client.SendAsync(request);
            var forecast = await BuildWeatherForecast(response, weatherRequest);
            return forecast;
        }

        private async Task<WeatherForecast> GetWeatherForecastForPeriod(WeatherRequest weatherRequest)
        {
            var weatherNow = await GetWeatherForecastForNow(weatherRequest);
            var request = new HttpRequestMessage(HttpMethod.Get, $"onecall?lat={weatherNow.Latitude}&lon={weatherNow.Longitude}&exclude=current,minutely,daily&appid={_key}");
            var response = await _client.SendAsync(request);
            var forecast = await BuildWeatherForecast(response, weatherNow);
            return forecast;
        }

        private async Task<WeatherForecast> BuildWeatherForecast(HttpResponseMessage response, WeatherForecast weatherNow)
        {
            var obj = new ForecastWeatherMapBase();
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                obj = await JsonSerializer.DeserializeAsync<ForecastWeatherMapBase>(stream);
            }
            return new WeatherForecast(obj, weatherNow);
        }

        private async Task<WeatherForecast> BuildWeatherForecast(HttpResponseMessage response,
            WeatherRequest weatherRequest)
        {
            var obj = new CurrentWeatherMapBase();
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                obj = await JsonSerializer.DeserializeAsync<CurrentWeatherMapBase>(stream);
            }
            
            return new WeatherForecast(obj, weatherRequest);
        }
    }
}
