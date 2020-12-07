using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CiphixAir.API.Models;
using CiphixAir.Core.Helpers;
using CiphixAir.Core.Models;

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
}
