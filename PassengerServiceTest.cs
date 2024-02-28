using Microsoft.Extensions.Logging;
using Moq;
using Simplifly.Interfaces;
using Simplifly.Models;
using Simplifly.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliflyTest
{
    internal class PassengerServiceTest
    {
        private PassengerService _passengerService;
        private Mock<IRepository<int, Passenger>> _mockPassengerRepository;
        private Mock<ILogger<PassengerService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockPassengerRepository = new Mock<IRepository<int, Passenger>>();
            _mockLogger = new Mock<ILogger<PassengerService>>();

            _passengerService = new PassengerService(_mockPassengerRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task AddPassenger_ValidPassenger_ReturnsAddedPassenger()
        {
            // Arrange
            var passenger = new Passenger
            {
                PassengerId = 1,
                Name = "John Doe",
                Age = 10,
                PassportNo = "2345"
            };
            _mockPassengerRepository.Setup(repo => repo.Add(passenger)).ReturnsAsync(passenger);

            // Act
            var addedPassenger = await _passengerService.AddPassenger(passenger);

            // Assert
            Assert.AreEqual(passenger, addedPassenger);
        }

        [Test]
        public async Task RemovePassenger_ExistingPassengerId_ReturnsTrue()
        {
            // Arrange
            var passengerId = 1;
            var passenger = new Passenger
            {
                PassengerId = passengerId,
                Name = "John Doe",
                Age = 10,
                PassportNo = "2345"
            };
            _mockPassengerRepository.Setup(repo => repo.GetAsync(passengerId)).ReturnsAsync(passenger);

            // Act
            var result = await _passengerService.RemovePassenger(passengerId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task RemovePassenger_NonExistingPassengerId_ReturnsFalse()
        {
            // Arrange
            var nonExistingPassengerId = 999;
            _mockPassengerRepository.Setup(repo => repo.GetAsync(nonExistingPassengerId)).ReturnsAsync((Passenger)null);

            // Act
            var result = await _passengerService.RemovePassenger(nonExistingPassengerId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetAllPassengers_ReturnsListOfPassengers()
        {
            // Arrange
            var Passenger = new Passenger();

            Passenger.Name = "John Doe";
            Passenger.Age = 10;
            Passenger.PassportNo = "2345";
            var passenger2 = new Passenger { Name = "John ", Age = 20, PassportNo = "22345" };

            var passengers = new List<Passenger> { Passenger, passenger2 };
            _mockPassengerRepository.Setup(repo => repo.GetAsync()).ReturnsAsync(passengers);

            // Act
            var result = await _passengerService.GetAllPassengers();

            // Assert
            Assert.AreEqual(passengers, result);
        }

        [Test]
        public async Task GetByIdPassengers_ExistingPassengerId_ReturnsPassenger()
        {
            // Arrange
            var passengerId = 1;
            var passenger = new Passenger { PassengerId = passengerId, Name = "John Doe", Age = 10, PassportNo = "2345" };
            _mockPassengerRepository.Setup(repo => repo.GetAsync(passengerId)).ReturnsAsync(passenger);

            // Act
            var result = await _passengerService.GetByIdPassengers(passengerId);

            // Assert
            Assert.AreEqual(passenger, result);
        }

        [Test]
        public async Task GetByIdPassengers_NonExistingPassengerId_ReturnsNull()
        {
            // Arrange
            var nonExistingPassengerId = 999;
            _mockPassengerRepository.Setup(repo => repo.GetAsync(nonExistingPassengerId)).ReturnsAsync((Passenger)null);

            // Act
            var result = await _passengerService.GetByIdPassengers(nonExistingPassengerId);

            // Assert
            Assert.IsNull(result);
        }
    }
}
