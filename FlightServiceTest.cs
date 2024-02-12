using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Simplifly.Context;
using Simplifly.Interfaces;
using Simplifly.Models;
using Simplifly.Repositories;
using Simplifly.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliflyTest
{
    internal class FlightServiceTest
    {
        RequestTrackerContext context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<RequestTrackerContext>().UseInMemoryDatabase("dummyDatabase").Options;
            context = new RequestTrackerContext(options);
        }

        [Test]
        [Order(3)]
        public async Task AddFlightTest()
        {
            var mockFlightRepositoryLogger = new Mock<ILogger<FlightRepository>>();
            var mockFlightServiceLogger = new Mock<ILogger<FlightService>>();

            IRepository<string, Flight> flightRepository = new FlightRepository(context, mockFlightRepositoryLogger.Object);
            
            IFlightFlightOwnerService flightOwnerService = new FlightService(flightRepository, mockFlightServiceLogger.Object);

            Flight flight = new Flight
            {
                FlightNumber = "IND99999",
                Airline = "Indigo",
                TotalSeats = 120,
                FlightOwnerOwnerId = 1
            };
            var flightTest = await flightOwnerService.AddFlight(flight);

            Assert.That(flightTest.FlightNumber, Is.EqualTo(flight.FlightNumber));
        }

        [Test]
        [Order(4)]
        public async Task GetAllFlightTest()
        {
            var mockFlightRepositoryLogger = new Mock<ILogger<FlightRepository>>();
            var mockFlightServiceLogger = new Mock<ILogger<FlightService>>();

            IRepository<string, Flight> flightRepository = new FlightRepository(context, mockFlightRepositoryLogger.Object);
            IFlightFlightOwnerService flightOwnerService = new FlightService(flightRepository, mockFlightServiceLogger.Object);

            var flights = await flightOwnerService.GetAllFlights();
            Assert.IsNotEmpty(flights);
        }


        [Test]
        [Order(5)]
        public async Task UpdateAirlineTest()
        {
            var mockFlightRepositoryLogger = new Mock<ILogger<FlightRepository>>();
            var mockFlightServiceLogger = new Mock<ILogger<FlightService>>();

            IRepository<string, Flight> flightRepository = new FlightRepository(context, mockFlightRepositoryLogger.Object);
            IFlightFlightOwnerService flightOwnerService = new FlightService(flightRepository, mockFlightServiceLogger.Object);

            Flight flight = new Flight
            {
                Airline = "AirIndia",
            };

            var updatedFlight = await flightOwnerService.UpdateAirline("IND99999", flight.Airline);
            Assert.That(updatedFlight.Airline, Is.EqualTo(flight.Airline));
        }

        [Test]
        [Order(6)]
        public async Task UpdateTotalSeatsTest()
        {
            var mockFlightRepositoryLogger = new Mock<ILogger<FlightRepository>>();
            var mockFlightServiceLogger = new Mock<ILogger<FlightService>>();

            IRepository<string, Flight> flightRepository = new FlightRepository(context, mockFlightRepositoryLogger.Object);
            IFlightFlightOwnerService flightOwnerService = new FlightService(flightRepository, mockFlightServiceLogger.Object);

            Flight flight = new Flight
            {
                TotalSeats = 80
            };

            var updatedFlight = await flightOwnerService.UpdateTotalSeats("IND99999", flight.TotalSeats);
            Assert.That(updatedFlight.TotalSeats, Is.EqualTo(flight.TotalSeats));
        }

        [Test]
        [Order(30)]
        public async Task RemoveFlightTest()
        {
            var mockFlightRepositoryLogger = new Mock<ILogger<FlightRepository>>();
            var mockFlightServiceLogger = new Mock<ILogger<FlightService>>();

            IRepository<string, Flight> flightRepository = new FlightRepository(context, mockFlightRepositoryLogger.Object);
            IFlightFlightOwnerService flightOwnerService = new FlightService(flightRepository, mockFlightServiceLogger.Object);

            var flight = await flightOwnerService.RemoveFlight("IND99999");
            Assert.That(flight.FlightNumber, Is.EqualTo("IND99999"));
        }
    }
}
