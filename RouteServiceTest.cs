using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Simplifly.Context;
using Simplifly.Interfaces;
using Simplifly.Models;
using Simplifly.Repositories;
using Simplifly.Services;
using Route = Simplifly.Models.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliflyTest
{
    internal class RouteServiceTest
    {
        RequestTrackerContext context;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<RequestTrackerContext>().UseInMemoryDatabase("dummyDatabase").Options;
            context = new RequestTrackerContext(options);
        }

        [Test]
        [Order(1)]
        public async Task AddRouteTest()
        {
            var mockRouteRepoLogger = new Mock<ILogger<RouteRepository>>();
            var mockRouteServiceLogger = new Mock<ILogger<RouteService>>();
            var mockAirportRepoLogger = new Mock<ILogger<AirportRepository>>();

            IRepository<int, Route> routeRepository = new RouteRepository(context, mockRouteRepoLogger.Object);
            IRouteFlightOwnerService routeService = new RouteService(routeRepository, mockRouteServiceLogger.Object);
            IRepository<int, Airport> airportRepository = new AirportRepository(context, mockAirportRepoLogger.Object);

            Airport sourceAirport = new Airport
            {
                Name = "Maharana Pratap Airport",
                City = "Udaipur",
                State = "Rajasthan",
                Country = "India"
            };
            var getSourceAirport = airportRepository.Add(sourceAirport);

            Airport destinationAirport = new Airport
            {
                Name = "Maharana Pratap Airport",
                City = "Udaipur",
                State = "Rajasthan",
                Country = "India"
            };
            var getDestinationAirport = airportRepository.Add(destinationAirport);

            Route route = new Route
            {
                SourceAirportId = getSourceAirport.Id,
                DestinationAirportId = getDestinationAirport.Id,
            };
            var routes = await routeService.AddRoute(route);
            Assert.That(getSourceAirport.Id, Is.EqualTo(routes.SourceAirportId));
        }

        [Test]
        [Order(2)]
        public async Task GetRouteTest()
        {
            var mockRouteRepoLogger = new Mock<ILogger<RouteRepository>>();
            var mockRouteServiceLogger = new Mock<ILogger<RouteService>>();

            IRepository<int, Route> routeRepository = new RouteRepository(context, mockRouteRepoLogger.Object);
            IRouteFlightOwnerService routeService = new RouteService(routeRepository, mockRouteServiceLogger.Object);

            var routes = await routeService.GetAllRoutes();
            Assert.IsNotEmpty(routes);
        }

        [Test]
        [Order(31)]
        public async Task RemoveRouteTest()
        {
            var mockRouteRepoLogger = new Mock<ILogger<RouteRepository>>();
            var mockRouteServiceLogger = new Mock<ILogger<RouteService>>();

            IRepository<int, Route> routeRepository = new RouteRepository(context, mockRouteRepoLogger.Object);
            IRouteFlightOwnerService routeService = new RouteService(routeRepository, mockRouteServiceLogger.Object);

            Route route = new Route
            {
                SourceAirportId = 2,
                DestinationAirportId = 1,
            };
            var routes = await routeService.AddRoute(route);

            var getRoute = await routeService.RemoveRoute(route.SourceAirportId,route.DestinationAirportId);
            Assert.That(getRoute.SourceAirportId, Is.EqualTo(route.SourceAirportId));
        }
    }
}
