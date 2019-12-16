using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DltcGeoServer.Models
{
    public class CustomFeature
    {
        public string type => "Feature";
        public Geometry geometry;
        public List<Property> properties => new List<Property>();
    }
}
