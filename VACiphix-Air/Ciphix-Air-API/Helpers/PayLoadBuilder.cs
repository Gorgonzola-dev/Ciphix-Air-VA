using System;
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
            var payload = new Struct
            {
                Fields =
                {
                    {"DateTime", Value.ForString(weather.Date.ToString("yyyy-MM-dd hh:mm:ss"))},
                    {"TemperatureCelsius", Value.ForNumber(weather.TemperatureC)},
                    {"TemperatureFahrenheit", Value.ForNumber(weather.TemperatureF)},
                    {"Summary", Value.ForString(weather.Summary)},
                }
            };
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
                stringBuilder.Append($"The weather in {weather.City} is {weather.Summary} with a temperature of {weather.TemperatureC}C");
                return stringBuilder.ToString();
            }

            if (weather.Date.Subtract(DateTime.UtcNow).TotalHours < 12)
            {
                stringBuilder.Append($"The weather in {weather.City} around {weather.Date:H:mm} will be {weather.Summary} with a temperature of {weather.TemperatureC}C");
                return stringBuilder.ToString();
            }

            stringBuilder.Append($"The weather in {weather.City} on {weather.Date:dddd, dd MMMM H:mm} will be {weather.Summary} with a temperature of {weather.TemperatureC}C");
            return stringBuilder.ToString();
        }
    }
}