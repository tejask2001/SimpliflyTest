using Castle.Core.Logging;
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
    internal class ScheduleServiceTest
    {
        RequestTrackerContext context;
        Schedule addedSchedule = new Schedule();

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<RequestTrackerContext>().UseInMemoryDatabase("dummyDatabase").Options;
            context = new RequestTrackerContext(options);

            
        }

        [Test]
        [Order(7)]
        public async Task AddScheduleTest()
        {
            var mockScheduleRepoLogger=new Mock<ILogger<ScheduleRepository>>();
            var mockScheduleServiceLogger=new Mock<ILogger<ScheduleServices>>();

            IRepository<int, Schedule> scheduleRepository = new ScheduleRepository(context, mockScheduleRepoLogger.Object);
            IScheduleFlightOwnerService scheduleService = new ScheduleServices(scheduleRepository, mockScheduleServiceLogger.Object);

            Schedule schedule = new Schedule
            {
                FlightId = "IND99998",
                RouteId = 1,
                Departure = new DateTime(2024, 3, 13, 16, 0, 0),
                Arrival = new DateTime(2024, 3, 13, 23, 0, 0)
            };
            addedSchedule= await scheduleService.AddSchedule(schedule);
            Assert.That(addedSchedule.FlightId, Is.EqualTo(schedule.FlightId));
        }

        [Test]
        [Order(8)]
        public async Task GetScheduleTest()
        {
            var mockScheduleRepoLogger = new Mock<ILogger<ScheduleRepository>>();
            var mockScheduleServiceLogger = new Mock<ILogger<ScheduleServices>>();

            IRepository<int, Schedule> scheduleRepository = new ScheduleRepository(context, mockScheduleRepoLogger.Object);
            IScheduleFlightOwnerService scheduleService = new ScheduleServices(scheduleRepository, mockScheduleServiceLogger.Object);

            var schedule=await scheduleService.GetAllSchedules();
            Assert.IsNotEmpty(schedule);
        }

        [Test]
        [Order(9)]
        public async Task UpdateScheduleFlightTest()
        {
            var mockScheduleRepoLogger = new Mock<ILogger<ScheduleRepository>>();
            var mockScheduleServiceLogger = new Mock<ILogger<ScheduleServices>>();

            IRepository<int, Schedule> scheduleRepository = new ScheduleRepository(context, mockScheduleRepoLogger.Object);
            IScheduleFlightOwnerService scheduleService = new ScheduleServices(scheduleRepository, mockScheduleServiceLogger.Object);

            Schedule updateSchedule = new Schedule
            {
                FlightId = "VIS444444"
            };
            var schedules = await scheduleService.UpdateScheduledFlight(addedSchedule.Id, updateSchedule.FlightId);
            Assert.That(updateSchedule.FlightId, Is.EqualTo(schedules.FlightId));
        }
    }
}
