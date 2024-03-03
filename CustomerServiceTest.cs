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
    internal class CustomerServiceTest
    {
        private CustomerService _customerService;
        private Mock<IRepository<int, Customer>> _mockCustomerRepository;
        private Mock<IRepository<string, User>> _mockUserRepository;
        private Mock<ILogger<CustomerService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockCustomerRepository = new Mock<IRepository<int, Customer>>();
            _mockUserRepository = new Mock<IRepository<string, User>>();
            _mockLogger = new Mock<ILogger<CustomerService>>();

            _customerService = new CustomerService(_mockCustomerRepository.Object, _mockUserRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task AddCustomer_ValidCustomer_ReturnsAddedCustomer()
        {
            // Arrange
            var customer = new Customer { UserId = 1, Name = "John Doe" };
            _mockCustomerRepository.Setup(repo => repo.Add(customer)).ReturnsAsync(customer);

            // Act
            var addedCustomer = await _customerService.AddCustomer(customer);

            // Assert
            Assert.AreEqual(customer, addedCustomer);
        }

        [Test]
        public async Task RemoveCustomer_ExistingCustomerId_ReturnsTrue()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer { UserId = customerId, Name = "John Doe", Username = "johndoe" };
            _mockCustomerRepository.Setup(repo => repo.Delete(customerId)).ReturnsAsync(customer);
            // _mockUserRepository.Setup(repo => repo.Delete(customer.Username)).ReturnsAsync(true);

            // Act
            var result = await _customerService.RemoveCustomer(customerId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task RemoveCustomer_NonExistingCustomerId_ReturnsFalse()
        {
            // Arrange
            var nonExistingCustomerId = 999;
            _mockCustomerRepository.Setup(repo => repo.Delete(nonExistingCustomerId)).ReturnsAsync((Customer)null);

            // Act
            var result = await _customerService.RemoveCustomer(nonExistingCustomerId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task UpdateCustomerEmail_ExistingCustomerId_ReturnsUpdatedCustomer()
        {
            // Arrange
            var customerId = 1;
            var updatedEmail = "john@example.com";
            var customer = new Customer { UserId = customerId, Name = "John Doe", Email = "oldemail@example.com" };
            _mockCustomerRepository.Setup(repo => repo.GetAsync(customerId)).ReturnsAsync(customer);
            _mockCustomerRepository.Setup(repo => repo.Update(customer)).ReturnsAsync(customer);

            // Act
            var updatedCustomer = await _customerService.UpdateCustomerEmail(customerId, updatedEmail);

            // Assert
            Assert.AreEqual(updatedEmail, updatedCustomer.Email);
        }

        [Test]
        public async Task UpdateCustomerEmail_NonExistingCustomerId_ReturnsNull()
        {
            // Arrange
            var nonExistingCustomerId = 999;
            _mockCustomerRepository.Setup(repo => repo.GetAsync(nonExistingCustomerId)).ReturnsAsync((Customer)null);

            // Act
            var updatedCustomer = await _customerService.UpdateCustomerEmail(nonExistingCustomerId, "test@example.com");

            // Assert
            Assert.IsNull(updatedCustomer);
        }

        [Test]
        public async Task GetByIdCustomers_ExistingCustomerId_ReturnsCustomer()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer { UserId = customerId, Name = "John Doe" };
            _mockCustomerRepository.Setup(repo => repo.GetAsync(customerId)).ReturnsAsync(customer);

            // Act
            var retrievedCustomer = await _customerService.GetByIdCustomers(customerId);

            // Assert
            Assert.AreEqual(customer, retrievedCustomer);
        }

        
    }
}
