using Microsoft.Extensions.Logging;
using Moq;
using Simplifly.Controllers;
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
    internal class BookingServiceTest
    {
        private BookingService _bookingService;
        private Mock<IRepository<int, Booking>> _mockBookingRepository;
        private Mock<IRepository<string, Flight>> _mockFlightRepository;
        private Mock<IRepository<int, PassengerBooking>> _mockPassengerBookingRepository;
        private Mock<ISeatDeatilRepository> _mockSeatDetailRepository;
        private Mock<IPassengerBookingRepository> _mockPassengerBookingsRepository;
        private Mock<IBookingRepository> _mockBookingsRepository;
        private Mock<IRepository<int, Payment>> _mockPaymentRepository;
        private Mock<IRepository<int, Schedule>> _mockScheduleRepository;
        //  private Mock<IPaymentRepository> _mockPaymentsRepository;
        private Mock<ILogger<BookingService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockBookingRepository = new Mock<IRepository<int, Booking>>();
            _mockFlightRepository = new Mock<IRepository<string, Flight>>();
            _mockPassengerBookingRepository = new Mock<IRepository<int, PassengerBooking>>();
            _mockSeatDetailRepository = new Mock<ISeatDeatilRepository>();
            _mockScheduleRepository = new Mock<IRepository<int, Schedule>>();
            _mockPassengerBookingsRepository = new Mock<IPassengerBookingRepository>();
            _mockBookingsRepository = new Mock<IBookingRepository>();
            _mockPaymentRepository = new Mock<IRepository<int, Payment>>();
            // _mockPaymentsRepository = new Mock<IPaymentRepository>();
            _mockLogger = new Mock<ILogger<BookingService>>();

            _bookingService = new BookingService(
                _mockBookingRepository.Object,
                _mockScheduleRepository.Object,
                _mockPassengerBookingRepository.Object,
                //_mockPaymentsRepository.Object,
                _mockFlightRepository.Object,
                _mockBookingsRepository.Object,
                _mockSeatDetailRepository.Object,
                _mockPassengerBookingsRepository.Object,
                _mockPaymentRepository.Object,
                _mockLogger.Object);
        }

        [Test]
        [Order(3)]
        public async Task CreateBookingAsync_ValidBookingRequest_ReturnsTrue()
        {
            // Arrange
            var bookingRequest = new BookingRequestDto
            {
                ScheduleId = 1,
                UserId = 1,
                SelectedSeats = new List<string> { "A1", "A2" },
                PassengerIds = new List<int> { 1, 2 },

            };
            Payment payment = new Payment { Amount = 200, PaymentDate = DateTime.Now, Status = PaymentStatus.Successful };
            var flight = new Flight { FlightNumber = "FL001", BasePrice = 100 };
            _mockFlightRepository.Setup(repo => repo.GetAsync("FL001")).ReturnsAsync(flight);
            _mockPassengerBookingsRepository.Setup(repo => repo.CheckSeatsAvailabilityAsync(1, It.IsAny<List<string>>())).ReturnsAsync(true);
            _mockSeatDetailRepository.Setup(repo => repo.GetSeatDetailsAsync(bookingRequest.SelectedSeats)).ReturnsAsync(new List<SeatDetail> { new SeatDetail { SeatNumber = "A1", SeatClass = "Economy" }, new SeatDetail { SeatNumber = "A2", SeatClass = "Business" } });
            _mockScheduleRepository.Setup(repo => repo.GetAsync(1)).ReturnsAsync(new Schedule {FlightId = "IND99999", Departure = DateTime.Now.AddDays(2), Arrival = DateTime.Now.AddDays(3),RouteId = 1});
            _mockBookingRepository.Setup(repo => repo.Add(It.IsAny<Booking>())).ReturnsAsync(new Booking { Id = 1, ScheduleId = 1, UserId = 1, BookingTime = DateTime.Now, TotalPrice = 200 });
            _mockPassengerBookingRepository.Setup(repo => repo.Add(It.IsAny<PassengerBooking>())).ReturnsAsync(new PassengerBooking { Id = 1, BookingId = 1, PassengerId = 1, SeatNumber = "A1" });
            _mockPaymentRepository.Setup(repo => repo.Add(It.IsAny<Payment>())).ReturnsAsync(new Payment { PaymentId = 1, Amount = 200, PaymentDate = DateTime.Now, Status = PaymentStatus.Successful });

            // Act
            var result = await _bookingService.CreateBookingAsync(bookingRequest);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task GetAllBookingsAsync_ReturnsListOfBookings()
        {
            // Arrange
            var bookings = new List<Booking> { new Booking { Id = 1, ScheduleId = 1, UserId = 1, BookingTime = DateTime.Now, TotalPrice = 200 } };
            _mockBookingRepository.Setup(repo => repo.GetAsync()).ReturnsAsync(bookings);

            // Act
            var result = await _bookingService.GetAllBookingsAsync();

            // Assert
            Assert.AreEqual(bookings, result);
        }

        [Test]
        public async Task GetBookingByIdAsync_ExistingBookingId_ReturnsBooking()
        {
            // Arrange
            var bookingId = 1;
            var booking = new Booking
            {
                Id = 1,
                ScheduleId = 1,
                UserId = 1,
                BookingTime = DateTime.Now,
                TotalPrice = 200
            };
            _mockBookingRepository.Setup(repo => repo.GetAsync(bookingId)).ReturnsAsync(booking);

            // Act
            var result = await _bookingService.GetBookingByIdAsync(bookingId);

            // Assert
            Assert.AreEqual(booking, result);
        }

        [Test]
        public async Task CancelBookingAsync_ExistingBookingId_ReturnsCancelledBooking()
        {
            // Arrange
            var bookingId = 1;
            var booking = new Booking { Id = 1, ScheduleId = 1, UserId = 1, BookingTime = DateTime.Now, TotalPrice = 200 };
            var payment = new Payment { PaymentId = 1, Amount = 200, PaymentDate = DateTime.Now, Status = PaymentStatus.Successful };
            _mockBookingRepository.Setup(repo => repo.GetAsync(bookingId)).ReturnsAsync(booking);
            _mockPaymentRepository.Setup(repo => repo.GetAsync(booking.PaymentId)).ReturnsAsync(payment);
            _mockPassengerBookingsRepository.Setup(repo => repo.GetPassengerBookingsAsync(bookingId)).ReturnsAsync(new List<PassengerBooking> { new PassengerBooking { Id = 1, BookingId = 1, PassengerId = 1, SeatNumber = "A1" } });
            _mockBookingRepository.Setup(repo => repo.Delete(bookingId)).ReturnsAsync(booking);
            _mockPaymentRepository.Setup(repo => repo.Delete(bookingId)).ReturnsAsync(payment);
            //  _mockPassengerBookingsRepository.Setup(repo => repo.Delete(It.IsAny<int>())).ReturnsAsync(true);
            _mockSeatDetailRepository.Setup(repo => repo.UpdateSeatDetailsAsync(It.IsAny<IEnumerable<SeatDetail>>())).Returns(Task.CompletedTask);

            // Act
            var result = await _bookingService.CancelBookingAsync(bookingId);

            // Assert
            Assert.AreEqual(booking, result);
        }

        [Test]
        public async Task GetUserBookingsAsync_ExistingUserId_ReturnsListOfBookings()
        {
            // Arrange
            var userId = 1;
            var bookings = new List<Booking> { new Booking { Id = 1, ScheduleId = 1, UserId = 1, BookingTime = DateTime.Now, TotalPrice = 200 } };
            _mockBookingsRepository.Setup(repo => repo.GetBookingsByUserIdAsync(userId)).ReturnsAsync(bookings);

            // Act
            var result = await _bookingService.GetUserBookingsAsync(userId);

            // Assert
            Assert.AreEqual(bookings, result);
        }

        [Test]
        public void CalculateTotalPrice_ValidParameters_ReturnsTotalPrice()
        {
            // Arrange
            var numberOfSeats = 2;
            var flight = new Flight { BasePrice = 100 };

            // Act
            var totalPrice = _bookingService.CalculateTotalPrice(numberOfSeats, flight);

            // Assert
            Assert.AreEqual(200, totalPrice);
        }

        [Test]
        public async Task RequestRefundAsync_ValidBookingId_ReturnsTrue()
        {
            // Arrange
            var bookingId = 1;
            var booking = new Booking { Id = bookingId };
            var payment = new Payment { PaymentId = 1, Amount = 200, PaymentDate = DateTime.Now, Status = PaymentStatus.Successful };
            _mockBookingRepository.Setup(repo => repo.GetAsync(bookingId)).ReturnsAsync(booking);
            _mockPaymentRepository.Setup(repo => repo.GetAsync(booking.PaymentId)).ReturnsAsync(payment);
            _mockPaymentRepository.Setup(repo => repo.Update(payment)).ReturnsAsync(payment);

            // Act
            var result = await _bookingService.RequestRefundAsync(bookingId);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
