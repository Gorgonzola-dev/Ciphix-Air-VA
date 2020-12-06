using System.Text.Json.Serialization;

namespace CiphixAir.Core.Models.AviationStack
{
    public class AviationFlightData
    {
        public string flight_date { get; set; }
        public string flight_status { get; set; }

        [JsonPropertyName("departure")]
        public AviationFlightDeparture AviationFlightDeparture { get; set; }

        [JsonPropertyName("arrival")]
        public AviationFlightArrival AviationFlightArrival { get; set; }
        public Airline airline { get; set; }

        [JsonPropertyName("flight")]
        public AviationFlight AviationFlight { get; set; }
        public object aircraft { get; set; }
        public object live { get; set; }
    }
}