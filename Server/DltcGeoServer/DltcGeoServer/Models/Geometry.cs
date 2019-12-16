using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DltcGeoServer.Models
{
    public class Geometry
    {
        public string type => "Point";
        public List<double> coordinates;
    }
}
