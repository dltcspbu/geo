using System.Collections.Generic;
using System.Linq;
using DltcGeoServer.Models;
using DltcGeoServer.Services;
using Itinero.Exceptions;
using Itinero.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace DltcGeoServer.Controllers
{
    [Route("api/Routes")]
    [ApiController]
    public class RoutesController : ControllerBase
    {
        private readonly RoutesService _routesService;
        public RoutesController(RoutesService routesService)
        {
            _routesService = routesService ?? new RoutesService();
        }

        // GET api/routes
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/routes/paired?vehicle=car&vehicle=pedestrian
        [HttpPost("paired")]
        public ActionResult FindRoute([FromQuery(Name = "vehicle")] List<string> vehicles, [FromBody] IEnumerable<Point> points)
        {
            if (points == null)
                return BadRequest("Failed to deserialize model");

            if (points.Count() < 2)
                return BadRequest("Number of points should be > 1");

            var profiles = new List<Profile>();
            foreach (var vehicle in vehicles)
            {
                switch (vehicle)
                {
                    case "car":
                        profiles.Add(Itinero.Osm.Vehicles.Vehicle.Car.Shortest());
                        break;
                    case "pedestrian":
                        profiles.Add(Itinero.Osm.Vehicles.Vehicle.Pedestrian.Shortest());
                        break;
                }
            }
            if (profiles.Count == 0)
                profiles.Add(Itinero.Osm.Vehicles.Vehicle.Car.Shortest());

            var route = new List<Point>();
            var start = points.First();

            foreach (var point in points.Skip(1))
            {
                try
                {
                    var routePoints = _routesService.GetPath(start, point, profiles);
                    if (routePoints != null)
                    {
                        route.AddRange(routePoints);                        
                    }
                    start = point;
                }
                catch (RouteNotFoundException e)
                {
                    switch (e.Message)
                    {
                        case "start":
                            start = point;
                            break;
                        default:
                            // Throw this point, don't change start point
                            break;

                    }                    
                }
            }

            return Ok(route);
        }

        // POST api/routes/grouped?vehicle=car&vehicle=pedestrian
        [HttpPost("grouped")]
        public ActionResult FindRouteForGroup([FromQuery(Name = "vehicle")] List<string> vehicles, [FromBody] List<Point> points)
        {
            if (points == null)
                return BadRequest("Failed to deserialize model");

            if (points.Count() < 2)
                return BadRequest("Number of points should be > 1");

            var profiles = new List<Profile>();
            foreach (var vehicle in vehicles)
            {
                switch (vehicle)
                {
                    case "car":
                        profiles.Add(Itinero.Osm.Vehicles.Vehicle.Car.Shortest());
                        break;
                    case "pedestrian":
                        profiles.Add(Itinero.Osm.Vehicles.Vehicle.Pedestrian.Shortest());
                        break;
                }
            }
            if (profiles.Count == 0)
                profiles.Add(Itinero.Osm.Vehicles.Vehicle.Car.Shortest());

            var route = _routesService.GetPathForGroup(points, profiles);

            return Ok(route);
        }
    }
}