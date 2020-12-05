using System;
using System.Collections.Generic;
using System.Text;

namespace CiphixAir.Core.Models.OpenWeatherMap.Forecast
{
    public class ForecastWeatherMapBase
    {
        public double lat { get; set; }
        public double lon { get; set; }
        public string timezone { get; set; }
        public int timezone_offset { get; set; }
        public List<ForecastWeatherHourly> hourly { get; set; }
        public List<ForecastWeatherDaily> daily { get; set; }
    }
}
