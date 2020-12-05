using System;
using System.Linq;
using System.Text;
using CiphixAir.API.Models;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf.WellKnownTypes;
using static Google.Cloud.Dialogflow.V2.Intent.Types;
using static Google.Cloud.Dialogflow.V2.Intent.Types.Message.Types;

namespace CiphixAir.API.Helpers
{
    public static class PayLoadBuilder
    {
        public static WebhookResponse BuildGoogleResponse(WeatherForecast weather)
        {
            var response = new WebhookResponse();

            var message = new Message();
            var text = new Text(); //You have to build up the message & text separately to prevent a NullRefException
            text.Text_.Add(BuildFulfillmentText(weather));
            message.Text = text;
            response.FulfillmentMessages.Add(message);
            var nightCelsiusValue = weather.TemperatureNightInCelsius.HasValue //Checking for value here because otherwise i'd need to send back 0 which would be inaccurate
                ? Value.ForNumber(weather.TemperatureNightInCelsius.Value)
                : Value.ForNull();       
            var nightFahrenheitValue = weather.TemperatureNightInFahrenheit.HasValue
                ? Value.ForNumber(weather.TemperatureNightInFahrenheit.Value)
                : Value.ForNull();

            var payload = new Struct
            {
                Fields =
                {
                    {"DateTime", Value.ForString(weather.DateTime.ToString("yyyy-MM-dd hh:mm:ss"))},
                    {"TemperatureDayInCelsius", Value.ForNumber(weather.TemperatureDayInCelsius)},
                    {"TemperatureNightInCelsius", nightCelsiusValue}, 
                    {"TemperatureDayInFahrenheit", nightFahrenheitValue},
                    {"TemperatureNightInFahrenheit", Value.ForNumber(weather.TemperatureNightInFahrenheit ?? 0)},
                    {"Summary", Value.ForString(weather.Summary)},
                }
            };
            response.FulfillmentText = response.FulfillmentMessages.First().Text.Text_.First(); //Google Documentation suggests using FulfillmentMessages.Text.Text_ for returning the Message but this results in an empty response in the DialogFlow, therefor this workaround
            response.Payload = payload;
            return response;
        }

        public static WebhookResponse BuildGoogleErrorResponse()
        {
            var response = new WebhookResponse();
            var message = new Message();
            var text = new Text(); //You have to build up the message & text separately to prevent a NullRefException
            text.Text_.Add(BuildFulfillmentErrorText());
            message.Text = text;
            response.FulfillmentMessages.Add(message);
            response.FulfillmentText = response.FulfillmentMessages.First().Text.Text_.First(); //Google Documentation suggests using FulfillmentMessages.Text.Text_ for returning the Message but this results in an empty response in the DialogFlow, therefor this workaround
            return response;
        }

        private static string BuildFulfillmentErrorText()
        {
            return "The current version sadly doesn't allow for Forecasts further than 2 days away";
        }


        private static string BuildFulfillmentText(WeatherForecast weather)
        {
            //TODO: Discuss with Marcel, is there no way to handle this in Google DialogFlow?
            var stringBuilder = new StringBuilder();
            if (!weather.IsForecast)
            {
                stringBuilder.Append($"The weather in {weather.City} is {weather.Summary} with a temperature of {weather.TemperatureDayInCelsius}C");
                return stringBuilder.ToString();
            }

            if (weather.DateTime.Subtract(DateTime.UtcNow).TotalHours < 12)
            {
                stringBuilder.Append($"The weather in {weather.City} around {weather.DateTime:H:mm} will be {weather.Summary} with a temperature of {weather.TemperatureDayInCelsius}C");
                return stringBuilder.ToString();
            }

            if (weather.TemperatureNightInCelsius != null)
            {
                stringBuilder.Append($"The weather in {weather.City} on {weather.DateTime:dddd, dd MMMM} during the day will be {weather.Summary} with a temperature of {weather.TemperatureDayInCelsius}C, During the night it will be {weather.TemperatureNightInCelsius} degrees Celsius");
                return stringBuilder.ToString();
            }

            stringBuilder.Append($"The weather in {weather.City} on {weather.DateTime:dddd, dd MMMM} during the day will will be {weather.Summary} with a temperature of {weather.TemperatureDayInCelsius}C");
            return stringBuilder.ToString();
        }
    }
}