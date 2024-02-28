// ... (previous using statements)

using Microsoft.Extensions.Logging;
using Moq;
using Simplifly.Exceptions;
using Simplifly.Interfaces;
using Simplifly.Models;
using Simplifly.Services;

namespace SimpliflyTest
{
    [TestFixture]
    internal class ScheduleServicesTest
    {
        private ScheduleServices _scheduleServices;
        private Mock<IRepository<int, Schedule>> _mockScheduleRepository;
        private Mock<IBookingService> _mockBookingService;
        private Mock<ILogger<ScheduleServices>> _mockLogger;

        [SetUp]
        public void Setup()
        {
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
