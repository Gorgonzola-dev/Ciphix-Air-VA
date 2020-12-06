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
        private readonly string _key;
        private readonly string _timezoneKey;

        public OpenWeatherMapService(string apiKey, string timezoneKey)
        {
            _key = apiKey;
            _timezoneKey = timezoneKey;
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

        public async Task<List<WeatherForecast>> GetWeatherForecastByFlight(FlightData flight, WeatherRequest weatherRequest)
        {
            var requestForDeparture= new WeatherRequest();
            requestForDeparture.City = flight.Departure.City;
            var requestForArrival= new WeatherRequest();
            requestForArrival.City = flight.Arrival.City;
            var arrival = await GetWeatherForecastForNow(requestForArrival);
            var departure= await GetWeatherForecastForNow(requestForArrival);
            var timezoneService = new GoogleTimeZoneService(_timezoneKey);

            var arrivalTimezone = await timezoneService.GetTimeZoneByWeatherForeCastAsync(arrival);
            var departureTimezone = await timezoneService.GetTimeZoneByWeatherForeCastAsync(departure);
            //This whole workaround is needed because the free tier of the AviationStack API doesn't allow me to get the city details and such easily, so improvisation was needed
            //When moving to full version of the AviationStack we can use https://api.aviationstack.com/v1/cities to get the gmt + or - and also the city_name

            var weatherList = new List<WeatherForecast>();
            //TODO Ronald: We should add the timezone.DayLightSavingOffset as well, but that's lower prio and don't know if it's as simple as just adding that to the TimezoneOffset
            requestForDeparture.DateTime = flight.Departure.DateTime.AddSeconds(departureTimezone.TimezoneOffset);
            requestForArrival.DateTime = flight.Arrival.DateTime.AddSeconds(arrivalTimezone.TimezoneOffset);
            
            var weatherForDeparture= await GetWeatherForecastForPeriod(requestForDeparture);
            var weatherForArrival= await GetWeatherForecastForPeriod(requestForArrival);

            weatherList.Add(weatherForDeparture);
            weatherList.Add(weatherForArrival);

            return weatherList;
        }
    }

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

    public class GoogleTimeZone
    {
        [JsonPropertyName("dstOffset")]
        public int DayLightSavingOffset { get; set; }
        [JsonPropertyName("rawOffset")]
        public int TimezoneOffset { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("timeZoneId")]
        public string TimezoneId { get; set; }
        [JsonPropertyName("timeZoneName")]
        public string TimezoneName { get; set; }
    }
}
