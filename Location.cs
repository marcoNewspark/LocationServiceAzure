using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationService
{
    public class Location
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string greeting { get; set; }

        public Location()
        {
            greeting = "Hello there.";
        }
    }

    public class InputAddress
    {
        public string address { get; set; }
    }
}
