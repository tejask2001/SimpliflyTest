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
        private Mock<ILogger<ScheduleServices>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<RequestTrackerContext>().UseInMemoryDatabase("dummyDatabase").Options;
            context = new RequestTrackerContext(options);

            _mockScheduleRepository = new Mock<IRepository<int, Schedule>>();
            _mockBookingService = new Mock<IBookingService>();
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
            var removedSchedule = await _scheduleServices.RemoveSchedule(schedule.FlightId);

            // Assert
            Assert.Greater(removedSchedule, 0);
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
