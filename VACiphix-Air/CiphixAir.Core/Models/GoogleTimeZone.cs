using System.Text.Json.Serialization;

namespace CiphixAir.Core.Services
{
    public class GoogleTimeZone
    {
        [JsonPropertyName("dstOffset")]
        public int DayLightSavingOffset { get; set; }
        [JsonPropertyName("rawOffset")]
        public int TimezoneOffset { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("timeZoneId")]
        public string TimezoneId { get; set; }
        [JsonPropertyName("timeZoneName")]
        public string TimezoneName { get; set; }
    }
}