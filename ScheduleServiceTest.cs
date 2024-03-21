// ... (previous using statements)

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Simplifly.Context;
using Simplifly.Exceptions;
using Simplifly.Interfaces;
using Simplifly.Models;
using Simplifly.Models.DTO_s;
using Simplifly.Repositories;
using Simplifly.Services;

namespace SimpliflyTest
{
    [TestFixture]
    internal class ScheduleServicesTest
    {
        RequestTrackerContext context;
        private ScheduleServices _scheduleServices;
        private Mock<IRepository<int, Schedule>> _mockScheduleRepository;
        private Mock<IBookingService> _mockBookingService;
        private Mock<IScheduleFlightOwnerService> _mockSchedule;
        private Mock<ILogger<ScheduleServices>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<RequestTrackerContext>().UseInMemoryDatabase("dummyDatabase").Options;
            context = new RequestTrackerContext(options);

            _mockScheduleRepository = new Mock<IRepository<int, Schedule>>();
            _mockBookingService = new Mock<IBookingService>();
            _mockSchedule = new Mock<IScheduleFlightOwnerService>();
            _mockLogger = new Mock<ILogger<ScheduleServices>>();

            _scheduleServices = new ScheduleServices(_mockScheduleRepository.Object, _mockBookingService.Object, _mockLogger.Object);
        }

        [Test]
        [Order(1)]
        public async Task AddScheduleTest()
        {
            // Arrange
            Schedule schedule = new Schedule
            {
                FlightId = "IND99999",
                Departure = DateTime.Now.AddDays(2),
                Arrival = DateTime.Now.AddDays(3),
                RouteId = 1
            };

            _mockScheduleRepository.Setup(repo => repo.GetAsync()).ReturnsAsync(new List<Schedule>());
            _mockScheduleRepository.Setup(repo => repo.Add(It.IsAny<Schedule>())).ReturnsAsync(schedule);

            // Act
            var addedSchedule = await _scheduleServices.AddSchedule(schedule);

            // Assert
            Assert.That(addedSchedule.FlightId, Is.EqualTo(schedule.FlightId));
        }

        [Test]
        [Order(8)]
        public async Task GetAllSchedulesTest()
        {
            // Arrange
            var schedulesList = new List<Schedule> { new Schedule { FlightId = "IND99999", Departure = DateTime.Now.AddDays(2), Arrival = DateTime.Now.AddDays(3), RouteId = 1 } };

            _mockScheduleRepository.Setup(repo => repo.GetAsync()).ReturnsAsync(schedulesList);

            // Act
            var schedules = await _scheduleServices.GetAllSchedules();

            // Assert
            Assert.IsNotEmpty(schedules);
        }

        

        [Test]
        public async Task RemoveScheduleTest()
        {
            // Arrange
            var schedule = new Schedule
            {
                Id = 1,
                FlightId = "Flight123",
                Departure = DateTime.UtcNow,
                Arrival = DateTime.UtcNow.AddHours(2)
            };

            _mockScheduleRepository.Setup(repo => repo.GetAsync(schedule.Id)).ReturnsAsync(schedule);
            _mockScheduleRepository.Setup(repo => repo.Delete(schedule.Id)).ReturnsAsync(schedule);

            // Act
            var removedSchedule = await _scheduleServices.RemoveSchedule(schedule);

            // Assert
            Assert.That(removedSchedule, Is.EqualTo(schedule));
        }

        [Test]
        public async Task RemoveScheduleByFlightNumberTest()
        {
            // Arrange
            var flightNumber = "Flight123";
            var schedule = new List<Schedule>
            {
            new Schedule{Id = 1, FlightId = flightNumber, Departure = DateTime.UtcNow,
            Arrival = DateTime.UtcNow.AddHours(2)}
            };

            _mockScheduleRepository.Setup(repo => repo.GetAsync()).ReturnsAsync(schedule);
            _mockScheduleRepository.Setup(repo => repo.Delete(It.IsAny<int>())).ReturnsAsync(new Schedule());

            // Act
            var removedSchedule = await _scheduleServices.RemoveSchedule(flightNumber);

            // Assert
            Assert.That(removedSchedule, Is.EqualTo(schedule.Count));
        }

        [Test]
        public async Task RemoveScheduleByDepartureDateTest()
        {
            // Arrange
            var departureDate = DateTime.UtcNow.Date;
            var airportId = 1;

            var schedulesToRemove = new List<Schedule>
            {
                new Schedule{Id = 1,FlightId = "Flight123",Departure = departureDate.AddHours(2),
            Arrival = departureDate.AddHours(4),Route = new Route { SourceAirportId = airportId }
                            }
            };

            _mockScheduleRepository.Setup(repo => repo.GetAsync()).ReturnsAsync(schedulesToRemove);
            _mockScheduleRepository.Setup(repo => repo.Delete(It.IsAny<int>())).ReturnsAsync(new Schedule());

            // Act
            var removedScheduleCount = await _scheduleServices.RemoveSchedule(departureDate, airportId);

            // Assert
            Assert.That(removedScheduleCount, Is.EqualTo(schedulesToRemove.Count));
        }


        [Test]
        public async Task UpdateScheduleFlight()
        {
            var scheduleToUpdate = new Schedule
            {
                Id = 1,
                FlightId = "AIR123",
                Departure = DateTime.UtcNow,
                Arrival = DateTime.UtcNow.AddHours(2),
                RouteId = 1
            };

            _mockScheduleRepository.Setup(repo => repo.GetAsync(scheduleToUpdate.Id)).ReturnsAsync(scheduleToUpdate);
            _mockScheduleRepository.Setup(repo => repo.Update(scheduleToUpdate)).ReturnsAsync(scheduleToUpdate);

            // Act
            var updatedSchedule = await _scheduleServices.UpdateScheduledFlight(scheduleToUpdate.Id, "AIR121212");

            // Assert
            Assert.That(updatedSchedule.FlightId, Is.EqualTo("AIR121212"));
        }

        [Test]
        public async Task UpdateScheduleRoute()
        {
            // Arrange
            var schedule = new Schedule
            {
                Id = 1,
                FlightId = "Flight123",
                Departure = DateTime.UtcNow,
                Arrival = DateTime.UtcNow.AddHours(2),
                RouteId=1
            };

            _mockScheduleRepository.Setup(repo => repo.GetAsync(schedule.Id)).ReturnsAsync(schedule);
            _mockScheduleRepository.Setup(repo => repo.Update(schedule)).ReturnsAsync(schedule);

            // Act
            var updatedSchedule = await _scheduleServices.UpdateScheduledRoute(schedule.Id, 2);

            // Assert
            Assert.That(updatedSchedule.RouteId, Is.EqualTo(2));
        }

        [Test]
        public async Task UpdateScheduleTime()
        {
            // Arrange
            var schedule = new Schedule
            {
                Id = 1,
                FlightId = "Flight123",
                Departure = DateTime.Now,
                Arrival = DateTime.Now.AddHours(2)
            };

            _mockScheduleRepository.Setup(repo => repo.GetAsync(schedule.Id)).ReturnsAsync(schedule);
            _mockScheduleRepository.Setup(repo => repo.Update(schedule)).ReturnsAsync(schedule);

            DateTime departure = DateTime.Now.AddHours(2);

            //Act
            var updateSchedule= await _scheduleServices.UpdateScheduledTime(schedule.Id,departure,DateTime.Now.AddHours(6));

            //Assert
            Assert.That(updateSchedule.Departure, Is.EqualTo(departure));
        }


        [Test]
        public void CalculateTotalPrice()
        {
            // Arrange
            var searchFlightDto = new SearchFlightDTO
            {
                SeatClass = "economy",
                Adult = 2,
                Child = 1
            };
            var basePrice = 1000.0;

            // Act
            var totalPrice = _scheduleServices.CalculateTotalPrice(searchFlightDto, basePrice);

            // Assert
            Assert.That(totalPrice, Is.EqualTo(4400.0));
        }

        [Test]
        public async Task GetFlightSchedules()
        {
            // Arrange
            var flightNumber = "FL123";
            var schedules = new List<Schedule>
            {
            new Schedule
            {
                Id = 1,
                FlightId = flightNumber,
                Route = new Route
                {
                    SourceAirport = new Airport { Name = "Raipur Airport", City = "Raipur" },
                    DestinationAirport = new Airport { Name = "Ahmadabad Airport", City = "Ahmadabad" }
                },
                Departure = DateTime.Now,
                Arrival = DateTime.Now.AddHours(2)
            }
            };
            _mockScheduleRepository.Setup(r => r.GetAsync()).ReturnsAsync(schedules);

            // Act
            var result = await _scheduleServices.GetFlightSchedules(flightNumber);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task SearchFlights_ReturnsCorrectSearchedFlightResults()
        {
            // Arrange
            var searchFlight = new SearchFlightDTO
            {
                DateOfJourney = DateTime.Now.Date,
                Origin = "SourceCity",
                Destination = "DestCity"
                // Add more properties as needed
            };

            var schedules = new List<Schedule>
    {
        new Schedule
        {
            Departure = DateTime.Now.Date,
            Route = new Route
            {
                SourceAirport = new Airport { City = "SourceCity" },
                DestinationAirport = new Airport { City = "DestCity" }
            },
            Flight = new Flight
            {
                FlightNumber = "IND123",
                Airline = "Indigo",
                BasePrice = 100.0,
                TotalSeats = 150
            }
        }
    };
            var mockScheduleRepository = new Mock<IRepository<int, Schedule>>();
            mockScheduleRepository.Setup(repo => repo.GetAsync()).ReturnsAsync(schedules);


            // Act
            var result = await _scheduleServices.SearchFlights(searchFlight);

            // Assert
            Assert.IsNotEmpty(result);
            // Add more assertions based on your actual logic
        }


        [Test]
        public async Task NoSuchScheduleExceptionTest()
        {
            // Arrange
            int scheduleId = 999;

            _mockScheduleRepository.Setup(repo => repo.GetAsync(scheduleId)).ThrowsAsync(new NoSuchScheduleException());

            // Act & Assert
            Assert.ThrowsAsync<NoSuchScheduleException>(async () => await _scheduleServices.UpdateScheduledFlight(scheduleId, "ABC123"));
        }

        

    }
}
