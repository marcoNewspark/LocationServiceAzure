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
    }

    public class InputAddress
    {
        public string address { get; set; }
    }
}
