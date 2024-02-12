using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Simplifly.Context;
using Simplifly.Exceptions;
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
            //Arrange
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
            //Act
            var flightTest = await flightOwnerService.AddFlight(flight);
            //Assert
            Assert.That(flightTest.FlightNumber, Is.EqualTo(flight.FlightNumber));
        }

        [Test]
        [Order(4)]
        public async Task GetAllFlightTest()
        {
            //Arrange
            var mockFlightRepositoryLogger = new Mock<ILogger<FlightRepository>>();
            var mockFlightServiceLogger = new Mock<ILogger<FlightService>>();

            IRepository<string, Flight> flightRepository = new FlightRepository(context, mockFlightRepositoryLogger.Object);
            IFlightFlightOwnerService flightOwnerService = new FlightService(flightRepository, mockFlightServiceLogger.Object);
            //Act
            var flights = await flightOwnerService.GetAllFlights();
            //Assert
            Assert.IsNotEmpty(flights);
        }


        [Test]
        [Order(5)]
        public async Task UpdateAirlineTest()
        {
            //Arrange
            var mockFlightRepositoryLogger = new Mock<ILogger<FlightRepository>>();
            var mockFlightServiceLogger = new Mock<ILogger<FlightService>>();

            IRepository<string, Flight> flightRepository = new FlightRepository(context, mockFlightRepositoryLogger.Object);
            IFlightFlightOwnerService flightOwnerService = new FlightService(flightRepository, mockFlightServiceLogger.Object);

            Flight flight = new Flight
            {
                Airline = "AirIndia",
            };
            //Act
            var updatedFlight = await flightOwnerService.UpdateAirline("IND99999", flight.Airline);
            //Assert
            Assert.That(updatedFlight.Airline, Is.EqualTo(flight.Airline));
        }

        [Test]
        [Order(6)]
        public async Task UpdateTotalSeatsTest()
        {
            //Arrange
            var mockFlightRepositoryLogger = new Mock<ILogger<FlightRepository>>();
            var mockFlightServiceLogger = new Mock<ILogger<FlightService>>();

            IRepository<string, Flight> flightRepository = new FlightRepository(context, mockFlightRepositoryLogger.Object);
            IFlightFlightOwnerService flightOwnerService = new FlightService(flightRepository, mockFlightServiceLogger.Object);

            Flight flight = new Flight
            {
                TotalSeats = 80
            };
            //Act
            var updatedFlight = await flightOwnerService.UpdateTotalSeats("IND99999", flight.TotalSeats);
            //Assert
            Assert.That(updatedFlight.TotalSeats, Is.EqualTo(flight.TotalSeats));
        }

        [Test]
        [Order(30)]
        public async Task RemoveFlightTest()
        {
            //Arrange
            var mockFlightRepositoryLogger = new Mock<ILogger<FlightRepository>>();
            var mockFlightServiceLogger = new Mock<ILogger<FlightService>>();

            IRepository<string, Flight> flightRepository = new FlightRepository(context, mockFlightRepositoryLogger.Object);
            IFlightFlightOwnerService flightOwnerService = new FlightService(flightRepository, mockFlightServiceLogger.Object);
            //Act
            var flight = await flightOwnerService.RemoveFlight("IND99999");
            //Assert
            Assert.That(flight.FlightNumber, Is.EqualTo("IND99999"));
        }

        [Test]
        public async Task NoSuchFlightExceptionTest()
        {
            //Arrange
            var mockFlightRepositoryLogger = new Mock<ILogger<FlightRepository>>();
            var mockFlightServiceLogger = new Mock<ILogger<FlightService>>();

            IRepository<string, Flight> flightRepository = new FlightRepository(context, mockFlightRepositoryLogger.Object);
            IFlightFlightOwnerService flightOwnerService = new FlightService(flightRepository, mockFlightServiceLogger.Object);
            //Act
            //Assert
            Assert.ThrowsAsync<NoSuchFlightException>(async () => await flightOwnerService.GetFlightById("AbC111"));
        }
    }
}
