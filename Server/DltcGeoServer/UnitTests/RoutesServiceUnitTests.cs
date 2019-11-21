using DltcGeoServer.Models;
using DltcGeoServer.Services;
using Itinero.Profiles;
using Microsoft.AspNetCore.Hosting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests
{
    [TestFixture]
    public class RoutesServiceUnitTests
    {
        private RoutesService _routesService;

        [SetUp]
        public void SetUp()
        {
            _routesService = new RoutesService();
        }

        [Test]
        public void GetPath_StartPointArgumentNullException_UnitTest()
        {
            // Arrange
            var profiles = new List<Profile>() { Itinero.Osm.Vehicles.Vehicle.Car.Shortest() };
            var endPoint = _endPointStub;

            // Act

            // Assert
            Assert.That(() => _routesService.GetPath(null, endPoint, profiles),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GetPath_EndPointArgumentNullException_UnitTest()
        {
            // Arrange
            var profiles = new List<Profile>() { Itinero.Osm.Vehicles.Vehicle.Car.Shortest() };
            var startPoint = _startPointStub;

            // Act

            // Assert
            Assert.That(() => _routesService.GetPath(startPoint, null, profiles),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void GetPath_EmptyProfilesList_UnitTest()
        {
            // Arrange
            var profiles = new List<Profile>();
            var startPoint = _startPointStub;
            var endPoint = _endPointStub;

            // Act
            var expected = _pathStub;
            var actual = _routesService.GetPath(startPoint, endPoint, profiles);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetPath_FindPairedPath_UnitTest()
        {
            // Arrange
            var profiles = new List<Profile>() { Itinero.Osm.Vehicles.Vehicle.Car.Shortest() };
            var startPoint = _startPointStub;
            var endPoint = _endPointStub;

            // Act
            var expected = _pathStub;
            var actual = _routesService.GetPath(startPoint, endPoint, profiles);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private Point _startPointStub => new Point { Latitude = 59.940231, Longitude = 30.268646 };
        private Point _endPointStub => new Point { Latitude = 59.940857, Longitude = 30.270785 };
        private List<Point> _pathStub => new List<Point>
        {
            new Point { Latitude = 59.940227508544922, Longitude = 30.268644332885742 },
            new Point { Latitude = 59.940250396728516, Longitude = 30.268732070922852 },
            new Point { Latitude = 59.9404296875, Longitude = 30.269344329833984 },
            new Point { Latitude = 59.940525054931641, Longitude = 30.269683837890625 },
            new Point { Latitude = 59.9405517578125, Longitude = 30.269763946533203 },
            new Point { Latitude = 59.940677642822266, Longitude = 30.270221710205078 },
            new Point { Latitude = 59.940692901611328, Longitude = 30.270252227783203 },
            new Point { Latitude = 59.940845489501953, Longitude = 30.270792007446289 }
        };
    }
}
