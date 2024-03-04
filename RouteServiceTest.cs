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
using Simplifly.Exceptions;

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
            //Arrange
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
            //Act
            var routes = await routeService.AddRoute(route);
            //Assert
            Assert.That(getSourceAirport.Id, Is.EqualTo(routes.SourceAirportId));
        }

        [Test]
        public async Task AddAirportTest()
        {
            //Arrange
            var mockRouteRepoLogger = new Mock<ILogger<RouteRepository>>();
            var mockAirportRepositoryLogger = new Mock<ILogger<AirportRepository>>();
            var mockRouteServiceLogger = new Mock<ILogger<RouteService>>();


            IRepository<int, Airport> airportRepository = new AirportRepository(context, mockAirportRepositoryLogger.Object);
            IRepository<int, Route> routeRepository = new RouteRepository(context, mockRouteRepoLogger.Object);
            IRouteFlightOwnerService routeService = new RouteService(routeRepository, mockRouteServiceLogger.Object,airportRepository);
            
            Airport airport = new Airport
            {
                Name = "Bhopal International Airport",
                City = "Bhopal",
                State = "Madhya Pradesh",
                Country = "India"
            };
            //Act
            var addedAirport = await routeService.AddAirport(airport);
            //Assert
            Assert.That(addedAirport.Name, Is.EqualTo(airport.Name));

        }

        [Test]
        public async Task GetAllAirportTest()
        {
            //Arrange
            var mockRouteRepoLogger = new Mock<ILogger<RouteRepository>>();
            var mockAirportRepositoryLogger = new Mock<ILogger<AirportRepository>>();
            var mockRouteServiceLogger = new Mock<ILogger<RouteService>>();


            IRepository<int, Airport> airportRepository = new AirportRepository(context, mockAirportRepositoryLogger.Object);
            IRepository<int, Route> routeRepository = new RouteRepository(context, mockRouteRepoLogger.Object);
            IRouteFlightOwnerService routeService = new RouteService(routeRepository, mockRouteServiceLogger.Object, airportRepository);
            //Act
            var airports = await routeService.GetAllAirports();
            //Assert
            Assert.IsNotEmpty(airports);
        }

        [Test]
        [Order(2)]
        public async Task GetRouteTest()
        {
            //Arrange
            var mockRouteRepoLogger = new Mock<ILogger<RouteRepository>>();
            var mockRouteServiceLogger = new Mock<ILogger<RouteService>>();

            IRepository<int, Route> routeRepository = new RouteRepository(context, mockRouteRepoLogger.Object);
            IRouteFlightOwnerService routeService = new RouteService(routeRepository, mockRouteServiceLogger.Object);
            //Act
            var routes = await routeService.GetAllRoutes();
            //Assert
            Assert.IsNotEmpty(routes);
        }

        [Test]
        public async Task GetRouteByIdTest()
        {
            //Arrange
            var mockRouteRepoLogger = new Mock<ILogger<RouteRepository>>();
            var mockRouteServiceLogger = new Mock<ILogger<RouteService>>();

            IRepository<int, Route> routeRepository = new RouteRepository(context, mockRouteRepoLogger.Object);
            IRouteFlightOwnerService routeService = new RouteService(routeRepository, mockRouteServiceLogger.Object);
            //Act
            Route route = new Route
            {
                SourceAirportId = 3,
                DestinationAirportId = 1
            };

            // Act
            var addRoute = await routeService.AddRoute(route);
            var getRouteById = await routeService.GetRouteById(addRoute.Id);

            // Assert
            Assert.That(getRouteById.Id, Is.EqualTo(addRoute.Id));

        }

        [Test]
        public async Task GetRouteIdByAirportTest()
        {
            //Arrange
            var mockRouteRepoLogger = new Mock<ILogger<RouteRepository>>();
            var mockRouteServiceLogger = new Mock<ILogger<RouteService>>();
            var mockAirportRepoLogger = new Mock<ILogger<AirportRepository>>();

            IRepository<int, Route> routeRepository = new RouteRepository(context, mockRouteRepoLogger.Object);
            IRouteFlightOwnerService routeService = new RouteService(routeRepository, mockRouteServiceLogger.Object);
            IRepository<int, Airport> airportRepository = new AirportRepository(context, mockAirportRepoLogger.Object);

            Airport sourceAirport = new Airport
            {
                Name = "Jabalpur Airport",
                City = "Jabalpur",
                State = "MP",
                Country = "India"
            };
            var getSourceAirport = airportRepository.Add(sourceAirport);

            Airport destinationAirport = new Airport
            {
                Name = "Agra Airport",
                City = "Agra",
                State = "UP",
                Country = "India"
            };
            var getDestinationAirport = airportRepository.Add(destinationAirport);

            Route route = new Route
            {
                SourceAirportId = getSourceAirport.Id,
                DestinationAirportId = getDestinationAirport.Id,
            };
            var routes = await routeService.AddRoute(route);

            //Act
            var getRoute = await routeService.GetRouteIdByAirport(getSourceAirport.Id, getDestinationAirport.Id);
            //Assert

            Assert.That(getRoute, Is.EqualTo(routes.Id));
        }


        [Test]
        [Order(29)]
        public async Task RemoveRouteTest()
        {
            //Arrange
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
            //Act
            var getRoute = await routeService.RemoveRoute(route.SourceAirportId,route.DestinationAirportId);
            //Assert
            Assert.That(getRoute.SourceAirportId, Is.EqualTo(route.SourceAirportId));
        }

        [Test]
        public async Task NoSuchRouteExceptionTest()
        {
            //Arrange
            var mockRouteRepoLogger = new Mock<ILogger<RouteRepository>>();
            var mockRouteServiceLogger = new Mock<ILogger<RouteService>>();

            IRepository<int, Route> routeRepository = new RouteRepository(context, mockRouteRepoLogger.Object);
            IRouteFlightOwnerService routeService = new RouteService(routeRepository, mockRouteServiceLogger.Object);

            //Act
            //Assert
            Assert.ThrowsAsync<NoSuchRouteException>(async () => await routeService.GetRouteById(12));
        }
    }
}
