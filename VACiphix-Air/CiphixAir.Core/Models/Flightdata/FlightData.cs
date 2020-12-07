namespace CiphixAir.Core.Models.Flightdata
{
    public class FlightData
    {
        public Departure Departure { get; set; } = new Departure();
        public Arrival Arrival { get; set; } = new Arrival();
        public string FlightCode { get; set; } //KLM1765
        public string FlightStatus { get; set; } //Landed
    }
}