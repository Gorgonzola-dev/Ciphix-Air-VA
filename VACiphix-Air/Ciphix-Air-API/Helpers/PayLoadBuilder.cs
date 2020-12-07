using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CiphixAir.API.Controllers;
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

            //Keeping message/text logic this here even though this doesn't work, this is how google Dialogflow docs suggests it should be done, so if they ever correct this this will be necessary
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

        public static WebhookResponse BuildGoogleErrorResponse(ErrorMessage errorMessage)
        {
            var response = new WebhookResponse();
            var message = new Message();
            var text = new Text(); //You have to build up the message & text separately to prevent a NullRefException
            text.Text_.Add(BuildFulfillmentErrorText(errorMessage));
            message.Text = text;
            response.FulfillmentMessages.Add(message);
            response.FulfillmentText = response.FulfillmentMessages.First().Text.Text_.First(); //Google Documentation suggests using FulfillmentMessages.Text.Text_ for returning the Message but this results in an empty response in the DialogFlow, therefor this workaround
            return response;
        }

        private static string BuildFulfillmentErrorText(ErrorMessage errorMessage)
        {
            switch (errorMessage)
            {
                case ErrorMessage.RequestedDateOutOfRange:
                    return "The current version sadly doesn't allow for Forecasts further than 7 days away";
                case ErrorMessage.NoFlightDataFound:
                    return "No Flight data was found for the requested Flight";
                default:
                    throw new ArgumentOutOfRangeException(nameof(errorMessage), errorMessage, null);
            }
            
        }


        private static string BuildFulfillmentText(WeatherForecast weather)
        {
            //TODO: Discuss with Marcel, is there no way to handle this in Google DialogFlow?
            var stringBuilder = new StringBuilder();
            if (!weather.IsForecast)
            {
                stringBuilder.Append($"The weather in {weather.City} is {weather.Summary} with a temperature of {weather.TemperatureDayInCelsius}\u2103");
                return stringBuilder.ToString();
            }

            if (weather.DateTime.Subtract(DateTime.UtcNow).TotalHours < 12)
            {
                stringBuilder.Append($"The weather in {weather.City} around {weather.DateTime:H:mm} will be {weather.Summary} with a temperature of {weather.TemperatureDayInCelsius}\u2103");
                return stringBuilder.ToString();
            }

            if (weather.TemperatureNightInCelsius != null)
            {
                stringBuilder.Append($"The weather in {weather.City} on {weather.DateTime:dddd, dd MMMM} during the day will be {weather.Summary} with a temperature of {weather.TemperatureDayInCelsius}\u2103, During the night it will be {weather.TemperatureNightInCelsius} degrees Celsius");
                return stringBuilder.ToString();
            }

            stringBuilder.Append($"The weather in {weather.City} on {weather.DateTime:dddd, dd MMMM} during the day will will be {weather.Summary} with a temperature of {weather.TemperatureDayInCelsius}\u2103");
            return stringBuilder.ToString();
        }

        public static WebhookResponse BuildGoogleResponse(List<WeatherForecast> weatherList)
        {
            var response = new WebhookResponse();
            //Keeping message/text logic this here even though this doesn't work, this is how google Dialogflow docs suggests it should be done, so if they ever correct this this will be necessary
            var message = new Message();
            var text = new Text(); //You have to build up the message & text separately to prevent a NullRefException
            text.Text_.Add(BuildFulfillmentText(weatherList));
            message.Text = text;
            response.FulfillmentMessages.Add(message);
            var payload = new Struct
            {
                Fields =
                {
                    {"DepartureDateTime", Value.ForString(weatherList[0].DateTime.ToString("yyyy-MM-dd hh:mm:ss"))},
                    {"DepartureTemperatureInCelsius", Value.ForNumber(weatherList[0].TemperatureDayInCelsius)},
                    {"DepartureTemperatureInFahrenheit", Value.ForNumber(weatherList[0].TemperatureDayInFahrenheit)},
                    {"DepartureSummary", Value.ForString(weatherList[0].Summary)},

                    {"ArrivalDateTime", Value.ForString(weatherList[1].DateTime.ToString("yyyy-MM-dd hh:mm:ss"))},
                    {"ArrivalTemperatureInCelsius", Value.ForNumber(weatherList[1].TemperatureDayInCelsius)},
                    {"ArrivalTemperatureInFahrenheit", Value.ForNumber(weatherList[1].TemperatureDayInFahrenheit)},
                    {"ArrivalSummary", Value.ForString(weatherList[1].Summary)},
                }
            };
            response.FulfillmentText = response.FulfillmentMessages.First().Text.Text_.First(); //Google Documentation suggests using FulfillmentMessages.Text.Text_ for returning the Message but this results in an empty response in the DialogFlow, therefor this workaround
            response.Payload = payload;
            return response;
        }

        private static string BuildFulfillmentText(List<WeatherForecast> weatherList)
        {
            return
                $"The weather in {weatherList[0].City} should be {weatherList[0].Summary} with a temperature of around {weatherList[0].TemperatureDayInCelsius}\u2103 when departing, When you arrive in {weatherList[1].City} the weather is {weatherList[1].Summary} with {weatherList[1].TemperatureDayInCelsius} degrees celsius";
        }
    }
}