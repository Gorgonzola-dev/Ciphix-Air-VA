using System;

namespace CiphixAir.Core.Models.AviationStack
{
    public class AviationFlightTimeData
    {
        public string airport { get; set; }
        public string timezone { get; set; }
        public int? delay { get; set; }
        public DateTime scheduled { get; set; }
        public DateTime estimated { get; set; }
        public DateTime? actual { get; set; }
        public DateTime? estimated_runway { get; set; }
        public DateTime? actual_runway { get; set; }
    }
}