using DltcGeoServer.Services;
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
    }
}
