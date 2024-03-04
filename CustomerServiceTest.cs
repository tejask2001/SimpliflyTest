using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Simplifly.Context;
using Simplifly.Interfaces;
using Simplifly.Models;
using Simplifly.Models.DTO_s;
using Simplifly.Repositories;
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
        RequestTrackerContext context;

        [SetUp]
        public void Setup()
        {
            _mockCustomerRepository = new Mock<IRepository<int, Customer>>();
            _mockUserRepository = new Mock<IRepository<string, User>>();
            _mockLogger = new Mock<ILogger<CustomerService>>();
            var options = new DbContextOptionsBuilder<RequestTrackerContext>().UseInMemoryDatabase("dummyDatabase").Options;
            context = new RequestTrackerContext(options);
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
        public async Task GetAllCustomersTest()
        {
            // Arrange
            var mockCustomerRepositoryLogger = new Mock<ILogger<CustomerRepository>>();
            var mockUserRepositoryLogger = new Mock<ILogger<UserRepository>>();
            var mockCustomerServiceLogger = new Mock<ILogger<CustomerService>>();

            IRepository<int, Customer> customerRepository = new CustomerRepository(context, mockCustomerRepositoryLogger.Object);
            IRepository<string, User> userRepository = new UserRepository(context, mockUserRepositoryLogger.Object);

            ICustomerService customerService = new CustomerService(customerRepository, userRepository, mockCustomerServiceLogger.Object);

            Customer customer1 = new Customer
            {
                Name = "Customer 1",
                Email = "customer1@example.com",
                Phone = "1111111111",
                Username = "user1"
            };

            await customerRepository.Add(customer1);

            // Act
            var customers = await customerService.GetAllCustomers();

            // Assert
            Assert.That(customers.Count, Is.EqualTo(1));
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

        [Test]
        public async Task GetCustomersByUsernameTest()
        {
            // Arrange
            var mockCustomerRepositoryLogger = new Mock<ILogger<CustomerRepository>>();
            var mockUserRepositoryLogger = new Mock<ILogger<UserRepository>>();
            var mockCustomerServiceLogger = new Mock<ILogger<CustomerService>>();

            IRepository<int, Customer> customerRepository = new CustomerRepository(context, mockCustomerRepositoryLogger.Object);
            IRepository<string, User> userRepository = new UserRepository(context, mockUserRepositoryLogger.Object);

            ICustomerService customerService = new CustomerService(customerRepository, userRepository, mockCustomerServiceLogger.Object);

            Customer customer = new Customer
            {
                Name = "Nishant",
                Email = "nishant@gmail.com",
                Phone = "99887788",
                Username = "nishant"
            };

            await customerRepository.Add(customer);

            // Act
            var getCustomer = await customerService.GetCustomersByUsername("nishant");

            // Assert
            Assert.That(getCustomer, Is.EqualTo(customer));
        }

        [Test]
        public async Task UpdateCustomerTest()
        {
            var mockCustomerRepositoryLogger= new Mock<ILogger<CustomerRepository>>();
            var mockUserRepositoryLogger= new Mock<ILogger<UserRepository>>();
            var mockCustomerServiceLogger = new Mock<ILogger<CustomerService>>();
            IRepository<int, Customer> customerRepository = new CustomerRepository(context, mockCustomerRepositoryLogger.Object);
            IRepository<string,User> userRepository=new UserRepository(context, mockUserRepositoryLogger.Object);

            ICustomerService customerService = new CustomerService(customerRepository, userRepository, mockCustomerServiceLogger.Object);
            
            Customer customer = new Customer
            {
                Name = "Nikhil",
                Email = "nikhil@gmail.com",
                Phone = "98989898",
                Username = "nikhil"
            };

            await customerRepository.Add(customer);

            var updatedCustomerDTO = new UpdateCustomerDTO
            {
                UserId = customer.UserId,
                Name = "Nikhil B",
                Email = "nikhilb@gmail.com.com",
                Phone = "9999666699"
            };

            // Act
            var updatedCustomer = await customerService.UpdateCustomer(updatedCustomerDTO);

            // Assert
            Assert.That(updatedCustomer.Name, Is.EqualTo(updatedCustomerDTO.Name));

        }
    }
}
