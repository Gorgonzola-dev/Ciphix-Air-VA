using System.Collections.Generic;
using System.Text;

namespace CiphixAir.Core.Models.AviationStack
{
    public class AviationStackData
    {
        public Pagination pagination { get; set; }
        public List<AviationFlightData> data { get; set; }
    }
    public class Pagination
    {
        public int limit { get; set; }
        public int offset { get; set; }
        public int count { get; set; }
        public int total { get; set; }
    }
}
