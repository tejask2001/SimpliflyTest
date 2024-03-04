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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace SimpliflyTest
{
    internal class FlightOwnerServiceTest
    {
        RequestTrackerContext context;
        FlightOwner addedFlightOwner= new FlightOwner();

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<RequestTrackerContext>().UseInMemoryDatabase("dummyDatabase").Options;
            context = new RequestTrackerContext(options);
        }

        [Order(1)]
        [Test]
        public async Task AddFlightOwnerTest()
        {
            // Arrange
            var mockFlightOwnerRepositoryLogger = new Mock<ILogger<FlightOwnerRepository>>();
            var mockUserRepositoryLogger = new Mock<ILogger<UserRepository>>();
            var mockFlightOwnerServiceLogger = new Mock<ILogger<FlightOwnerService>>();

            IRepository<int, FlightOwner> flightOwnerRepository = new FlightOwnerRepository(context, mockFlightOwnerRepositoryLogger.Object);
            IRepository<string, User> userRepository = new UserRepository(context, mockUserRepositoryLogger.Object);

            IFlightOwnerService flightOwnerService = new FlightOwnerService(flightOwnerRepository, userRepository, mockFlightOwnerServiceLogger.Object);

            User user = new User
            {
                Username = "tejas",
                Password = new byte[] { 0x01, 0x02, 0x03, 0x04 },
                Role = "flightOwner",
                Key = new byte[] { 0xAA, 0xBB, 0xCC, 0xDD }
            };
            await userRepository.Add(user);
            FlightOwner flightOwner = new FlightOwner
            {
                Name= "Tejas",
                Email="tejas@gmail.com",
                CompanyName="Abc inl",
                ContactNumber="78978979",
                Address="Banglore",
                BusinessRegistrationNumber="4444555",
                Username="tejas"
                
            };

            // Act
            addedFlightOwner = await flightOwnerService.AddFlightOwner(flightOwner);

            // Assert
            Assert.That(addedFlightOwner.OwnerId, Is.EqualTo(flightOwner.OwnerId));
        }

        [Test]
        [Order(2)]
        public async Task GetAllFlightOwnerTest()
        {
            // Arrange
            var mockFlightOwnerRepositoryLogger = new Mock<ILogger<FlightOwnerRepository>>();
            var mockUserRepositoryLogger = new Mock<ILogger<UserRepository>>();
            var mockFlightOwnerServiceLogger = new Mock<ILogger<FlightOwnerService>>();

            IRepository<int, FlightOwner> flightOwnerRepository = new FlightOwnerRepository(context, mockFlightOwnerRepositoryLogger.Object);
            IRepository<string, User> userRepository = new UserRepository(context, mockUserRepositoryLogger.Object);

            IFlightOwnerService flightOwnerService = new FlightOwnerService(flightOwnerRepository, userRepository, mockFlightOwnerServiceLogger.Object);
            //Act
            var flightOwners = await flightOwnerService.GetAllFlightOwners();
            //Assert
            Assert.IsNotEmpty(flightOwners);
        }

        [Test]
        [Order(3)]
        public async Task UpdateFlightOwnerAddressTest()
        {
            // Arrange
            var mockFlightOwnerRepositoryLogger = new Mock<ILogger<FlightOwnerRepository>>();
            var mockUserRepositoryLogger = new Mock<ILogger<UserRepository>>();
            var mockFlightOwnerServiceLogger = new Mock<ILogger<FlightOwnerService>>();

            IRepository<int, FlightOwner> flightOwnerRepository = new FlightOwnerRepository(context, mockFlightOwnerRepositoryLogger.Object);
            IRepository<string, User> userRepository = new UserRepository(context, mockUserRepositoryLogger.Object);

            IFlightOwnerService flightOwnerService = new FlightOwnerService(flightOwnerRepository, userRepository, mockFlightOwnerServiceLogger.Object);
            
            FlightOwner addedFlightOwner = new FlightOwner
            {
                Address = "Mohali"
            };
            //Act
            var updatedFlightOwner = await flightOwnerService.UpdateFlightOwnerAddress(1, addedFlightOwner.Address);
            //Assert
            Assert.That(addedFlightOwner.Address, Is.EqualTo(updatedFlightOwner.Address));
        }

        [Test]
        [Order(4)]
        public async Task RemoveFlightOwnerTest()
        {
            // Arrange
            var mockFlightOwnerRepositoryLogger = new Mock<ILogger<FlightOwnerRepository>>();
            var mockUserRepositoryLogger = new Mock<ILogger<UserRepository>>();
            var mockFlightOwnerServiceLogger = new Mock<ILogger<FlightOwnerService>>();

            IRepository<int, FlightOwner> flightOwnerRepository = new FlightOwnerRepository(context, mockFlightOwnerRepositoryLogger.Object);
            IRepository<string, User> userRepository = new UserRepository(context, mockUserRepositoryLogger.Object);

            IFlightOwnerService flightOwnerService = new FlightOwnerService(flightOwnerRepository, userRepository, mockFlightOwnerServiceLogger.Object);
            //Act
            var deletedFlightOwner = await flightOwnerService.RemoveFlightOwner(1);
            //Assert
            Assert.That(true, Is.EqualTo(deletedFlightOwner));
        }

        [Test]
        public async Task NoSuchFlightOwnerExceptionTest()
        {
            // Arrange
            var mockFlightOwnerRepositoryLogger = new Mock<ILogger<FlightOwnerRepository>>();
            var mockUserRepositoryLogger = new Mock<ILogger<UserRepository>>();
            var mockFlightOwnerServiceLogger = new Mock<ILogger<FlightOwnerService>>();

            IRepository<int, FlightOwner> flightOwnerRepository = new FlightOwnerRepository(context, mockFlightOwnerRepositoryLogger.Object);
            IRepository<string, User> userRepository = new UserRepository(context, mockUserRepositoryLogger.Object);

            IFlightOwnerService flightOwnerService = new FlightOwnerService(flightOwnerRepository, userRepository, mockFlightOwnerServiceLogger.Object);
            //Act
            //Assert
            Assert.ThrowsAsync<NoSuchFlightOwnerException>(async () => await flightOwnerService.GetFlightOwnerById(11));
        }

        [Test]
        public async Task GetFlightOwnerByUsername()
        {
            // Arrange
            var mockFlightOwnerRepositoryLogger = new Mock<ILogger<FlightOwnerRepository>>();
            var mockUserRepositoryLogger = new Mock<ILogger<UserRepository>>();
            var mockFlightOwnerServiceLogger = new Mock<ILogger<FlightOwnerService>>();

            IRepository<int, FlightOwner> flightOwnerRepository = new FlightOwnerRepository(context, mockFlightOwnerRepositoryLogger.Object);
            IRepository<string, User> userRepository = new UserRepository(context, mockUserRepositoryLogger.Object);

            IFlightOwnerService flightOwnerService = new FlightOwnerService(flightOwnerRepository, userRepository, mockFlightOwnerServiceLogger.Object);
            //Act
            User user = new User
            {
                Username = "nikhil",
                Password = new byte[] { 0x01, 0x02, 0x03, 0x04 },
                Role = "flightOwner",
                Key = new byte[] { 0xAA, 0xAB, 0xCC, 0xDD }
            };
            await userRepository.Add(user);
            FlightOwner flightOwner = new FlightOwner
            {
                Name = "nikhil",
                Email = "nikhil@gmail.com",
                CompanyName = "Abc inl",
                ContactNumber = "78978979",
                Address = "Banglore",
                BusinessRegistrationNumber = "4444555",
                Username = "nikhil"

            };

            // Act
            addedFlightOwner = await flightOwnerService.AddFlightOwner(flightOwner);

            var getFlightOwner=await flightOwnerService.GetByUsernameFlightOwners(user.Username);
            //Assert
            Assert.That(getFlightOwner.Username, Is.EqualTo(user.Username));
        }

        [Test]
        public async Task GetFlightOwnerById()
        {
            // Arrange
            var mockFlightOwnerRepositoryLogger = new Mock<ILogger<FlightOwnerRepository>>();
            var mockUserRepositoryLogger = new Mock<ILogger<UserRepository>>();
            var mockFlightOwnerServiceLogger = new Mock<ILogger<FlightOwnerService>>();

            IRepository<int, FlightOwner> flightOwnerRepository = new FlightOwnerRepository(context, mockFlightOwnerRepositoryLogger.Object);
            IRepository<string, User> userRepository = new UserRepository(context, mockUserRepositoryLogger.Object);

            IFlightOwnerService flightOwnerService = new FlightOwnerService(flightOwnerRepository, userRepository, mockFlightOwnerServiceLogger.Object);
            //Act
            User user = new User
            {
                Username = "nishant",
                Password = new byte[] { 0x01, 0x02, 0x03, 0x04 },
                Role = "flightOwner",
                Key = new byte[] { 0xAA, 0xBB, 0xCC, 0xDD }
            };
            await userRepository.Add(user);
            FlightOwner flightOwner = new FlightOwner
            {
                Name = "nishant",
                Email = "nishant@gmail.com",
                CompanyName = "Abc inl",
                ContactNumber = "78978979",
                Address = "Banglore",
                BusinessRegistrationNumber = "4444555",
                Username = "nishant"

            };

            // Act
            addedFlightOwner = await flightOwnerService.AddFlightOwner(flightOwner);

            var getFlightOwner = await flightOwnerService.GetFlightOwnerById(addedFlightOwner.OwnerId);
            //Assert
            Assert.That(getFlightOwner.OwnerId, Is.EqualTo(addedFlightOwner.OwnerId));
        }

        [Test]
        public async Task UpdateFlightOwnerTest()
        {
            // Arrange
            var mockFlightOwnerRepositoryLogger = new Mock<ILogger<FlightOwnerRepository>>();
            var mockUserRepositoryLogger = new Mock<ILogger<UserRepository>>();
            var mockFlightOwnerServiceLogger = new Mock<ILogger<FlightOwnerService>>();

            IRepository<int, FlightOwner> flightOwnerRepository = new FlightOwnerRepository(context, mockFlightOwnerRepositoryLogger.Object);
            IRepository<string, User> userRepository = new UserRepository(context, mockUserRepositoryLogger.Object);

            IFlightOwnerService flightOwnerService = new FlightOwnerService(flightOwnerRepository, userRepository, mockFlightOwnerServiceLogger.Object);
            //Act
            User user = new User
            {
                Username = "Sarang",
                Password = new byte[] { 0x01, 0x02, 0x03, 0x04 },
                Role = "flightOwner",
                Key = new byte[] { 0xAA, 0xBB, 0xCC, 0xDD }
            };
            await userRepository.Add(user);
            FlightOwner flightOwner = new FlightOwner
            {
                Name = "Sarang",
                Email = "sarang@gmail.com",
                CompanyName = "Abc inl",
                ContactNumber = "78978979",
                Address = "Banglore",
                BusinessRegistrationNumber = "4444555",
                Username = "Sarang"

            };

            // Act
            addedFlightOwner = await flightOwnerService.AddFlightOwner(flightOwner);

            UpdateFlightOwnerDTO updateFlightOwner = new UpdateFlightOwnerDTO()
            {
                OwnerId = addedFlightOwner.OwnerId,
                Address = "Jamnagar",
                Email = "hello@outlook.com"
            };

            var updatedDetails=await flightOwnerService.UpdateFlightOwner(updateFlightOwner);

            Assert.AreEqual(updateFlightOwner.Address, updatedDetails.Address);
        }
    }
}
