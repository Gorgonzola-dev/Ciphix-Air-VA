using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CiphixAir.API.Models;
using CiphixAir.Core.Models;
using CiphixAir.Core.Services;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Enum = System.Enum;

namespace CiphixAir.API.Controllers
{
    [ApiController]
    [Route("weather")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _configuration;


        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
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
            
            var response = new WebhookResponse();
            if (Enum.TryParse<IntentType>(request.OriginalDetectIntentRequest.Source, out var intent))
                return response;

            if (intent == IntentType.GetWeather)
            {
                var weatherService = new WeatherService(_configuration["OpenWeatherApiKey"]);
                var payloadBuilderService = new PayLoadBuilderService();
                //request.QueryResult.Parameters.Fields
                var weatherRequest = JsonSerializer.Deserialize<WeatherRequest>(request.QueryResult.Parameters.ToString());
                var weather = await weatherService.GetWeatherForecast(weatherRequest);
                response.Payload = payloadBuilderService.BuildGooglePayload(weather);
            }
            return response;
        }



        enum IntentType
        {
            GetWeather
        }
    }

}

public class PayLoadBuilderService
{

    public Struct BuildGooglePayload(WeatherForecast weather)
    {
        var payload = new Struct
        {
            Fields =
            {
                {"DateTime", Value.ForString(weather.Date.ToString("yyyy-MM-dd hh:mm:ss"))},
                {"TemperatureCelsius", Value.ForNumber(weather.TemperatureF)},
                {"TemperatureFahrenheit", Value.ForNumber(weather.TemperatureF)},
                {"Summary", Value.ForString(weather.Summary)},
            }
        };
        return payload;
    }
}

