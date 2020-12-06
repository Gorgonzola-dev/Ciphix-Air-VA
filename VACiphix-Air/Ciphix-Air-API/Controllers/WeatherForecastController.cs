using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CiphixAir.API.Helpers;
using CiphixAir.API.Models;
using CiphixAir.Core.Models;
using CiphixAir.Core.Services;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CiphixAir.API.Controllers
{
    [ApiController]
    [Route("weather")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _configuration;
        private OpenWeatherMapService _weatherService;
        private AviationStackFlightService _flightService;


        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _weatherService = new OpenWeatherMapService(_configuration["OpenWeatherApiKey"], _configuration["GoogleTimezoneApiKey"]);
            _flightService = new AviationStackFlightService(_configuration["AviationstackApiKey"]);
        }
        private static readonly JsonParser Parser = new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));


        [HttpPost]
        public async Task<WebhookResponse> Post()
        {
            string requestJson;
            using (TextReader reader = new StreamReader(Request.Body))
            {
                requestJson = await reader.ReadToEndAsync();
            }
            var request = Parser.Parse<WebhookRequest>(requestJson);
            
            var weatherRequest = JsonSerializer.Deserialize<WeatherRequest>(request.QueryResult.Parameters.ToString());
            weatherRequest.Intent = Enum.Parse<IntentType>(request.QueryResult.Intent.DisplayName);
            if (weatherRequest.DateTime.Subtract(DateTime.UtcNow).TotalHours > 168)
            {
                return PayLoadBuilder.BuildGoogleErrorResponse();
            }
            WeatherForecast weather;
            WeatherRequest originalRequest;
            switch (weatherRequest.Intent)
            {
                case IntentType.GetWeatherForLocation:
                case IntentType.LocationGivenAfterWeatherRequestNoDateTime:
                    weather = await _weatherService.GetWeatherForecastForNow(weatherRequest);
                    break;
                case IntentType.OnlyFlightGivenAffirmationWeatherDesired:
                case IntentType.GetWeatherByFlight:
                    if (weatherRequest.Intent == IntentType.OnlyFlightGivenAffirmationWeatherDesired)
                    {
                        originalRequest = JsonSerializer.Deserialize<WeatherRequest>(request.QueryResult.OutputContexts[0].Parameters
                            .ToString());
                        weatherRequest.FlightProvider = originalRequest.FlightProvider;
                        weatherRequest.ForFlight = originalRequest.ForFlight;
                    }
                    var flight = await _flightService.GetFlightData(weatherRequest);
                    if (flight == null)
                    {
                        return PayLoadBuilder.BuildGoogleErrorResponse(); // TODO: Add more error responses
                    }
                    //weatherList always contains 2 weatherForecasts, the first is for Departure, the second for Arrival
                    var weatherList = await _weatherService.GetWeatherForecastByFlight(flight, weatherRequest);
                    var byFlightResponse = PayLoadBuilder.BuildGoogleResponse(weatherList);
                    return byFlightResponse;
                case IntentType.GetWeatherForDateTimeAndCity:
                case IntentType.LocationGivenAfterWeatherRequestWithDateTime:
                    weather = await _weatherService.GetWeatherForecastForPeriod(weatherRequest);
                    break;
                case IntentType.LocationGivenAfterWeatherRequestWithPreviousDateTime:
                    originalRequest = JsonSerializer.Deserialize<WeatherRequest>(request.QueryResult.OutputContexts[0].Parameters
                        .ToString());
                    weatherRequest.DateTime = originalRequest.DateTime;
                    weather = await _weatherService.GetWeatherForecastForPeriod(weatherRequest);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var response = PayLoadBuilder.BuildGoogleResponse(weather);

            return response;
        }
    }

}