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

namespace CiphixAir.Core.Services
{
    public class WeatherStackService
    {
        HttpClient _client = new HttpClient();
        private string _key;
        public WeatherStackService(string apiKey)
        {
            _key = apiKey;
            _client.BaseAddress = new Uri("http://api.weatherstack.com/");

        }
        //public async Task<WeatherForecast> GetWeatherForecast(WeatherRequest weatherRequest)
        //{
        //    if (weatherRequest.DateTimeGiven)
        //    {
        //        return await GetWeatherForecastForPeriod(weatherRequest);
        //    }

        //    return await GetWeatherForecastForNow(weatherRequest);
        //}

        //private async Task<WeatherForecast> GetWeatherForecastForNow(WeatherRequest weatherRequest)
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Get, $"current?access_key={_key}&query={weatherRequest.City}");
        //    var response = await _client.SendAsync(request);
        //    var forecast = await BuildWeatherForecast(response);
        //    return forecast;
        //}

        //private async Task<WeatherForecast> GetWeatherForecastForPeriod(WeatherRequest weatherRequest)
        //{
        //    var timeDifference = GetTimeFrame(weatherRequest);
        //    if (timeDifference == TimeSpan.Zero)
        //    {
        //        return await GetWeatherForecastForNow(weatherRequest);
        //    }

        //    if (timeDifference.TotalDays > 4)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    if (timeDifference.TotalDays > 1)
        //    {
        //        //GetWeatherForecastForLater()
        //    }
        //    else
        //    {
        //        return await GetWeatherForecastForToday(weatherRequest, timeDifference);
        //    }

        //    var forecastDays = timeDifference.TotalDays;
            

        //    var request = new HttpRequestMessage(HttpMethod.Get, $"forecast?access_key={_key}&query={weatherRequest.City}");
        //    var response = await _client.SendAsync(request);
        //    var forecast = await BuildWeatherForecast(response);
        //    return forecast;
        //}

        //private async Task<WeatherForecast> GetWeatherForecastForToday(WeatherRequest weatherRequest, TimeSpan timeDifference)
        //{
        //    var forecaseHour = timeDifference.Hours;
        //    var request = new HttpRequestMessage(HttpMethod.Get, $"forecast?access_key={_key}&query={weatherRequest.City}");
        //}

        //private TimeSpan GetTimeFrame(WeatherRequest weatherRequest)
        //{
        //    var now = DateTime.UtcNow;
        //    var timeDifference = weatherRequest.DateTime.Subtract(now);
        //    if (!(timeDifference.CompareTo(TimeSpan.Zero) > 0)) // Check if the requested dateTime is higher than Now
        //    {
        //        return TimeSpan.Zero;
        //    }

        //    return timeDifference;
        //}

        //private async Task<WeatherForecast> BuildWeatherForecast(HttpResponseMessage response)
        //{
        //    var obj = new CurrentWeatherMapBase();
        //    using (var stream = await response.Content.ReadAsStreamAsync())
        //    {
        //        obj = await JsonSerializer.DeserializeAsync<CurrentWeatherMapBase>(stream);
        //    }
            
        //    return new WeatherForecast(obj);
        //}
    }
}
