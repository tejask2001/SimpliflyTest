using Microsoft.Extensions.Logging;
using Moq;
using Simplifly.Interfaces;
using Simplifly.Models.DTOs;
using Simplifly.Models;
using Simplifly.Exceptions;
using Simplifly.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient.Server;
using Simplifly.Models.DTO_s;

namespace SimpliflyTest
{
    internal class UserServiceTest
    {
        private USerService _userService;
        private Mock<IRepository<string, User>> _mockUserRepository;
        private Mock<IRepository<int, Admin>> _mockAdminRepository;
        private Mock<IRepository<int, FlightOwner>> _mockFlightOwnerRepository;
        private Mock<IRepository<int, Customer>> _mockCustomerRepository;
        private Mock<ITokenService> _mockTokenService;
        private Mock<ILogger<USerService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IRepository<string, User>>();
            _mockAdminRepository = new Mock<IRepository<int, Admin>>();
            _mockFlightOwnerRepository = new Mock<IRepository<int, FlightOwner>>();
            _mockCustomerRepository = new Mock<IRepository<int, Customer>>();
            _mockTokenService = new Mock<ITokenService>();
            _mockLogger = new Mock<ILogger<USerService>>();

            _userService = new USerService(
                _mockUserRepository.Object,
                _mockAdminRepository.Object,
                _mockFlightOwnerRepository.Object,
                _mockCustomerRepository.Object,
                _mockTokenService.Object,
                _mockLogger.Object
            );
        }

        

        [Test]
        public async Task RegisterAdminTest()
        {
            // Arrange
            var admin = new RegisterAdminUserDTO
            {
                Username = "jack",
                Password = "password",
                Role = "admin",
                Name = "Jack",
                Email = "jack@gmail.com",
                Position = "Manager",
                ContactNumber = "85858585",
                Address = "Kochi"
            };

            var existingUsers = new List<User>
            {
                new User { Username = "Satyam" }
            };

            _mockUserRepository.Setup(repo => repo.GetAsync()).ReturnsAsync(existingUsers);
            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>()))
                .ReturnsAsync((User user) => user);
            _mockAdminRepository.Setup(repo => repo.Add(It.IsAny<Admin>()))
                .ReturnsAsync(new Admin());

            // Act
            var register = await _userService.RegisterAdmin(admin);

            // Assert
            Assert.That(register.Username, Is.EqualTo(admin.Username));
        }

        [Test]
        public async Task UpdatePasswordTest()
        {
            // Arrange
            var forgotPasswordDTO = new ForgotPasswordDTO
            {
                Username = "Satyam",
                Password = "newPassword",
            };

            var existingUser = new User
            {
                Username = forgotPasswordDTO.Username,
            };

            _mockUserRepository.Setup(repo => repo.GetAsync(forgotPasswordDTO.Username))
                .ReturnsAsync(existingUser);

            _mockUserRepository.Setup(repo => repo.Update(It.IsAny<User>()))
                .ReturnsAsync((User user) => user);

            // Act
            var result = await _userService.UpdateUserPassword(forgotPasswordDTO);

            // Assert
            Assert.AreEqual(forgotPasswordDTO.Username, result.Username);
            Assert.AreEqual(existingUser.Role, result.Role);
        }

        [Test]
        public async Task RegisterFlightOwnerTest()
        {
            // Arrange
            var flightOwnerUser = new RegisterFlightOwnerUserDTO
            {
                Username = "Testxyz",
                Password = "password",
                Role = "flightowner",
            };

            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>()))
                .ReturnsAsync((User user) => user);
            _mockFlightOwnerRepository.Setup(repo => repo.Add(It.IsAny<FlightOwner>()))
                .ReturnsAsync(new FlightOwner());

            // Act
            var result = await _userService.RegisterFlightOwner(flightOwnerUser);

            // Assert
            Assert.That(result.Username, Is.EqualTo(flightOwnerUser.Username));
        }

        [Test]
        public async Task RegisterCustomerTest()
        {
            // Arrange
            var customerUser = new RegisterCustomerUserDTO
            {
                Username = "TestPQR",
                Password = "password",
                Role = "customer",
            };

            _mockUserRepository.Setup(repo => repo.Add(It.IsAny<User>()))
                .ReturnsAsync((User user) => user);

            _mockCustomerRepository.Setup(repo => repo.Add(It.IsAny<Customer>()))
                .ReturnsAsync(new Customer());

            // Act
            var result = await _userService.RegisterCustomer(customerUser);

            // Assert
            Assert.That(result.Username, Is.EqualTo(customerUser.Username));
        }

        [Test]
        public async Task ComparePasswordTest()
        {
            byte[] password = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            byte[] userPassword = new byte[] { 0x01, 0x02, 0x03, 0x04 };

            var IsCorrect= _userService.ComparePasswords(password, userPassword);
            Assert.AreEqual(IsCorrect, true);
        }

        [Test]
        public void InvalidUserExceptionTest()
        {
            // Arrange
            var loginUserDTO = new LoginUserDTO
            {
                Username = "Zebra",
                Password = "password",
                Role = "customer    "
            };

            _mockUserRepository.Setup(repo => repo.GetAsync("Zebra")).ReturnsAsync((User)null);

            // Act & Assert
            Assert.ThrowsAsync<InvlidUuserException>(async () => await _userService.Login(loginUserDTO));
        }


    }
}
