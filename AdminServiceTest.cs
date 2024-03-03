using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Simplifly.Context;
using Simplifly.Exceptions;
using Simplifly.Interfaces;
using Simplifly.Models.DTO_s;
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
    internal class AdminServiceTest
    {
        private RequestTrackerContext context;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<RequestTrackerContext>().UseInMemoryDatabase("dummyDatabase").Options;
            context = new RequestTrackerContext(options);
        }

        [Test]
        public async Task DeleteUserTest()
        {
            // Arrange
            var mockAdminRepositoryLogger = new Mock<ILogger<AdminRepository>>();
            var mockUserRepositoryLogger = new Mock<ILogger<UserRepository>>();
            var mockAdminServiceLogger = new Mock<ILogger<AdminService>>();

            IRepository<int, Admin> adminRepository = new AdminRepository(context, mockAdminRepositoryLogger.Object);
            IRepository<string, User> userRepository = new UserRepository(context, mockUserRepositoryLogger.Object);

            IAdminService adminService = new AdminService(adminRepository, mockAdminServiceLogger.Object, userRepository);

            User user = new User
            {
                Username = "testuser",
                Password = new byte[] { 0x01, 0x02, 0x03, 0x04 },
                Role = "user",
                Key = new byte[] { 0xAA, 0xBB, 0xCC, 0xDD }
            };
            await userRepository.Add(user);

            // Act
            var deletedUser = await adminService.DeleteUser("testuser");

            // Assert
            Assert.That(deletedUser, Is.EqualTo(user));
        }

        [Test]
        public void DeleteUserNoSuchUserExceptionTest()
        {
            // Arrange
            var mockAdminRepositoryLogger = new Mock<ILogger<AdminRepository>>();
            var mockUserRepositoryLogger = new Mock<ILogger<UserRepository>>();
            var mockAdminServiceLogger = new Mock<ILogger<AdminService>>();

            IRepository<int, Admin> adminRepository = new AdminRepository(context, mockAdminRepositoryLogger.Object);
            IRepository<string, User> userRepository = new UserRepository(context, mockUserRepositoryLogger.Object);

            IAdminService adminService = new AdminService(adminRepository, mockAdminServiceLogger.Object, userRepository);

            // Act and Assert
            Assert.ThrowsAsync<NoSuchUserException>(async () => await adminService.DeleteUser("Chirag"));
        }

        [Test]
        public async Task GetAdminByUsernameTest()
        {
            // Arrange
            var mockAdminRepositoryLogger = new Mock<ILogger<AdminRepository>>();
            var mockUserRepositoryLogger = new Mock<ILogger<UserRepository>>();
            var mockAdminServiceLogger = new Mock<ILogger<AdminService>>();

            IRepository<int, Admin> adminRepository = new AdminRepository(context, mockAdminRepositoryLogger.Object);
            IRepository<string, User> userRepository = new UserRepository(context, mockUserRepositoryLogger.Object);

            IAdminService adminService = new AdminService(adminRepository, mockAdminServiceLogger.Object, userRepository);

            Admin admin = new Admin
            {
                Username = "AdminUser",
                Name = "AdminUser",
                Email = "adminUser@outlook.com"
            };
            await adminRepository.Add(admin);

            // Act
            var retrievedAdmin = await adminService.GetAdminByUsername("AdminUser");

            // Assert
            Assert.That(retrievedAdmin, Is.EqualTo(admin));
        }

        [Test]
        public void GetAdminByUsernameNoSuchAdminExceptionTest()
        {
            // Arrange
            var mockAdminRepositoryLogger = new Mock<ILogger<AdminRepository>>();
            var mockUserRepositoryLogger = new Mock<ILogger<UserRepository>>();
            var mockAdminServiceLogger = new Mock<ILogger<AdminService>>();

            IRepository<int, Admin> adminRepository = new AdminRepository(context, mockAdminRepositoryLogger.Object);
            IRepository<string, User> userRepository = new UserRepository(context, mockUserRepositoryLogger.Object);

            IAdminService adminService = new AdminService(adminRepository, mockAdminServiceLogger.Object, userRepository);

            // Act and Assert
            Assert.ThrowsAsync<NoSuchAdminException>(async () => await adminService.GetAdminByUsername("Devratna"));
        }

        [Test]
        public async Task UpdateAdminTest()
        {
            // Arrange
            var mockAdminRepositoryLogger = new Mock<ILogger<AdminRepository>>();
            var mockUserRepositoryLogger = new Mock<ILogger<UserRepository>>();
            var mockAdminServiceLogger = new Mock<ILogger<AdminService>>();

            IRepository<int, Admin> adminRepository = new AdminRepository(context, mockAdminRepositoryLogger.Object);
            IRepository<string, User> userRepository = new UserRepository(context, mockUserRepositoryLogger.Object);

            IAdminService adminService = new AdminService(adminRepository, mockAdminServiceLogger.Object, userRepository);

            Admin admin = new Admin
            {
                Username = "testadmin",
                Name = "Test Admin",
                Email = "testadmin@example.com"
            };
            await adminRepository.Add(admin);

            UpdateAdminDTO updatedAdmin = new UpdateAdminDTO
            {
                AdminId = admin.AdminId,
                Name = "Admin_John",
                Email = "adminJohn@email.com",
                Address = "Pune",
                ContactNumber = "9876543210",
                Position = "Manager"
            };

            // Act
            var result = await adminService.UpdateAdmin(updatedAdmin);

            // Assert
            Assert.That(result.Name, Is.EqualTo(updatedAdmin.Name));
        }

        [Test]
        public void UpdateAdminNoSuchAdminExceptionTest()
        {
            // Arrange
            var mockAdminRepositoryLogger = new Mock<ILogger<AdminRepository>>();
            var mockUserRepositoryLogger = new Mock<ILogger<UserRepository>>();
            var mockAdminServiceLogger = new Mock<ILogger<AdminService>>();

            IRepository<int, Admin> adminRepository = new AdminRepository(context, mockAdminRepositoryLogger.Object);
            IRepository<string, User> userRepository = new UserRepository(context, mockUserRepositoryLogger.Object);

            IAdminService adminService = new AdminService(adminRepository, mockAdminServiceLogger.Object, userRepository);

            UpdateAdminDTO updatedAdmin = new UpdateAdminDTO
            {
                AdminId = 999, // Assuming there is no admin with this ID
                Name = "admin_kane",
                Email = "jane@gmail.com",
                Address = "Chennai",
                ContactNumber = "9876543210",
                Position = "Supervisor"
            };

            // Act and Assert
            Assert.ThrowsAsync<NoSuchAdminException>(async () => await adminService.UpdateAdmin(updatedAdmin));
        }
    }
}
