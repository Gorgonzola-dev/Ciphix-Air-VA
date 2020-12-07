using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CiphixAir.Core.Models;
using CiphixAir.Core.Models.AviationStack;
using CiphixAir.Core.Models.Flightdata;

namespace CiphixAir.Core.Services
{
    public class AviationStackFlightService
    {
        HttpClient _client = new HttpClient();
        private string _key;
        public AviationStackFlightService(string key)
        {
            _key = key;
            var uri = new Uri("http://api.aviationstack.com/v1/flights");
            _client.BaseAddress = uri;
        }

        public async Task<FlightData> GetFlightData(WeatherRequest weatherRequestForFlight)
        {
            try
            {
                var flightProvider = weatherRequestForFlight.FlightProvider;
                var flight = weatherRequestForFlight.ForFlight;
                var request = new HttpRequestMessage(HttpMethod.Get, $"?airline_name={flightProvider}&flight_number={flight}&access_key={_key}");
                var response = await _client.SendAsync(request);

                var obj = new AviationStackData();
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    obj = await JsonSerializer.DeserializeAsync<AviationStackData>(stream);
                }

                var flightData = FlightDataBuilder.BuildFlightData(obj);
                return flightData;
            }
            catch(Exception e)
            {
                return null;
            }


        }
    }

    public class FlightDataBuilder
    {
        public static FlightData BuildFlightData(AviationStackData aviationStackData)
        {
            var flightData = new FlightData();
            if (aviationStackData.data.Count < 1)
                return null;
            var aviationFlightData = aviationStackData.data.First();

            flightData.FlightCode = aviationFlightData.AviationFlight.icao;
            flightData.FlightStatus = aviationFlightData.flight_status;
            flightData.Departure.DateTime = aviationFlightData.AviationFlightDeparture.estimated;
            flightData.Arrival.DateTime = aviationFlightData.AviationFlightArrival.estimated;
            
            //For a paid version of this API i would've simply invoked the city endpoint to get everything including Lon/Lat for invoking the Weather/Timezone API's.
            //This takes ~3 calls to get which would put me over the free usage limit too quickly
            var v = new Regex(@"(?<=\/)\w+");
            var departureCity = v.Match(aviationFlightData.AviationFlightDeparture.timezone).Value.Replace('_', ' ');
            var arrivalCity = v.Match(aviationFlightData.AviationFlightArrival.timezone).Value.Replace('_', ' ');

            flightData.Departure.City = departureCity;
            flightData.Arrival.City = arrivalCity;

            return flightData;
        }
    }
}
