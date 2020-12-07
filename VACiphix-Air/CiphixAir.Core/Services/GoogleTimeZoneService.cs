using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CiphixAir.API.Models;

namespace CiphixAir.Core.Services
{
    public class GoogleTimeZoneService
    {
        private readonly string _key;
        private readonly HttpClient _client;
        public GoogleTimeZoneService(string key)
        {
            _key = key;
            var timezoneUri = new Uri("https://maps.googleapis.com/maps/api/timezone/json");
            _client = new HttpClient();
            _client.BaseAddress = timezoneUri;
        }

        public async Task<GoogleTimeZone> GetTimeZoneByWeatherForeCastAsync(WeatherForecast arrival)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"?location={arrival.Latitude},{arrival.Longitude}key={_key}");
            var response = await _client.SendAsync(request);
            var timeZone = new GoogleTimeZone();
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                timeZone = await JsonSerializer.DeserializeAsync<GoogleTimeZone>(stream);
            }

            return timeZone;
        }
    }
}