namespace CiphixAir.Core.Models
{
    public enum IntentType
    {
        GetWeatherForLocation,
        GetWeatherByFlight,
        GetWeatherForDateTimeAndCity,
        LocationGivenAfterWeatherRequestWithPreviousDateTime,
        LocationGivenAfterWeatherRequestNoDateTime,
        LocationGivenAfterWeatherRequestWithDateTime,
        OnlyFlightGivenAffirmationWeatherDesired
    }
}