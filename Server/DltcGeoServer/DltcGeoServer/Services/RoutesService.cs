using DltcGeoServer.Models;
using Itinero;
using Itinero.Exceptions;
using Itinero.IO.Osm;
using Itinero.Profiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DltcGeoServer.Services
{
    public class RoutesService
    {
        private readonly RouterDb _routerDb;
        private readonly Router _router;

        public RoutesService()
        {
            _routerDb = new RouterDb();

            var stream = new FileStream("/app/LO.pbf", FileMode.Open);
            {
                try
                {
                    _routerDb.LoadOsmData(stream, new[]
                    {
                        Itinero.Osm.Vehicles.Vehicle.Car,
                        Itinero.Osm.Vehicles.Vehicle.Pedestrian
                    });
                    _router = new Router(_routerDb);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        public IEnumerable<Point> GetPath(Point start, Point end, List<Profile> profiles)
        {
            Route resultRoute = null;

            foreach (var profile in profiles)
            {
                var startEdge = _router.TryResolve(profile, (float)start.Latitude, (float)start.Longitude, searchDistanceInMeter: 10);
                var endEdge = _router.TryResolve(profile, (float)end.Latitude, (float)end.Longitude, searchDistanceInMeter: 10);
                if (startEdge.IsError)
                    throw new RouteNotFoundException("start");
                if(endEdge.IsError)
                    throw new RouteNotFoundException("end");

                try
                {
                    var route = _router.Calculate(profile, startEdge.Value, endEdge.Value);
                    if (resultRoute == null || route.TotalDistance < resultRoute.TotalDistance)
                        resultRoute = route;
                }
                catch (RouteNotFoundException e)
                {
                    Debug.WriteLine($"Route was not found between: ({start.Latitude} {start.Longitude}) and ({end.Latitude} {end.Longitude})");
                }
                catch (ResolveFailedException e)
                {
                    Debug.WriteLine($"Point was thrown: {end.Latitude} {end.Longitude}");
                }
            }

            if (resultRoute != null && resultRoute.TotalDistance > (GetMinLength(start, end) * 2))
                throw new RouteNotFoundException("Wrong target point");

            return resultRoute
                ?.Shape
                ?.Select(p => new Point
                {
                    Latitude = p.Latitude,
                    Longitude = p.Longitude
                });
        }

        public IEnumerable<Point> GetPathForGroup(List<Point> points, List<Profile> profiles)
        {
            Route resultRoute = null;

            foreach (var profile in profiles)
            {
                try
                {
                    var route = _router.Calculate(profile, points.Select(p => new Itinero.LocalGeo.Coordinate((float)p.Latitude, (float)p.Longitude)).ToArray());
                    if (resultRoute == null || resultRoute.TotalDistance < route.TotalDistance)
                        resultRoute = route;
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Failed to build route");
                }
            }

            return resultRoute
                ?.Shape
                ?.Select(p => new Point
                {
                    Latitude = p.Latitude,
                    Longitude = p.Longitude
                });
        }

        private static double GetMinLength(Point start, Point end)
        {
            var p = Math.PI / 180;
            var a = 0.5 - Math.Cos((end.Latitude - start.Latitude) * p) / 2 + Math.Cos(start.Latitude * p) * Math.Cos(end.Latitude * p) * (1 - Math.Cos((end.Longitude - start.Longitude) * p)) / 2;
            return 12742 * Math.Asin(Math.Sqrt(Math.Abs(a))) * 1000;
        }
    }
}