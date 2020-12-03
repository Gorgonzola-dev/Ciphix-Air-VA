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

namespace CiphixAir.Core.Services
{
    public class WeatherService
    {
        HttpClient _client = new HttpClient();
        private string _key;
        public WeatherService(string apiKey)
        {
            _key = apiKey;
            _client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/weather");

        }
        public async Task<WeatherForecast> GetWeatherForecast(WeatherRequest weatherRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"?q={weatherRequest.City}&appid={_key}");
            //TODO: Add Time check
            var response = await _client.SendAsync(request);
            var forecast = await BuildWeatherForecast(response);
            return forecast;
        }

        private async Task<WeatherForecast> BuildWeatherForecast(HttpResponseMessage response)
        {
            var obj = new OpenWeatherMapBase();
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                obj = await JsonSerializer.DeserializeAsync<OpenWeatherMapBase>(stream);
            }
            
            return new WeatherForecast(obj);
        }
    }
}
