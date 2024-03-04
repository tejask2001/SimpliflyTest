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
    internal class SeatDetailServiceTest
    {
        private SeatDetailService _seatDetailService;
        private Mock<IRepository<string, SeatDetail>> _mockSeatDetailRepository;
        private Mock<ILogger<SeatDetailService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockSeatDetailRepository = new Mock<IRepository<string, SeatDetail>>();
            _mockLogger = new Mock<ILogger<SeatDetailService>>();

            _seatDetailService = new SeatDetailService(_mockSeatDetailRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task AddSeatDetail_ValidSeatDetail_ReturnsAddedSeatDetail()
        {
            // Arrange
            var seatDetail = new SeatDetail { SeatNumber = "A1", SeatClass = "Economy" };
            _mockSeatDetailRepository.Setup(repo => repo.Add(seatDetail)).ReturnsAsync(seatDetail);

            // Act
            var addedSeatDetail = await _seatDetailService.AddSeatDetail(seatDetail);

            // Assert
            Assert.That(addedSeatDetail, Is.EqualTo(seatDetail));
        }

        [Test]
        public async Task RemoveSeatDetail_ExistingSeatDetailId_ReturnsTrue()
        {
            // Arrange
            var seatDetailId = "A1";
            var seatDetail = new SeatDetail { SeatNumber = seatDetailId, SeatClass = "Economy" };
            _mockSeatDetailRepository.Setup(repo => repo.GetAsync(seatDetailId)).ReturnsAsync(seatDetail);

            // Act
            var result = await _seatDetailService.RemoveSeatDetail(seatDetailId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task RemoveSeatDetail_NonExistingSeatDetailId_ReturnsFalse()
        {
            // Arrange
            var nonExistingSeatDetailId = "B2";
            _mockSeatDetailRepository.Setup(repo => repo.GetAsync(nonExistingSeatDetailId)).ReturnsAsync((SeatDetail)null);

            // Act
            var result = await _seatDetailService.RemoveSeatDetail(nonExistingSeatDetailId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetAllSeatDetails_ReturnsListOfSeatDetails()
        {
            var seatDetail = new SeatDetail { SeatNumber = "B1", SeatClass = "Economy" };
            var seatDetail1 = new SeatDetail { SeatNumber = "B3", SeatClass = "Economy" };
            // Arrange
            var seatDetails = new List<SeatDetail> { seatDetail, seatDetail1 };
            _mockSeatDetailRepository.Setup(repo => repo.GetAsync()).ReturnsAsync(seatDetails);

            // Act
            var result = await _seatDetailService.GetAllSeatDetails();

            // Assert
            Assert.That(result, Is.EqualTo(seatDetails));
        }

        [Test]
        public async Task GetByIdSeatDetails_ExistingSeatDetailId_ReturnsSeatDetail()
        {
            // Arrange
            var seatDetailId = "A1";
            var seatDetail = new SeatDetail { SeatNumber = seatDetailId, SeatClass = "Economy" };
            _mockSeatDetailRepository.Setup(repo => repo.GetAsync(seatDetailId)).ReturnsAsync(seatDetail);

            // Act
            var result = await _seatDetailService.GetByIdSeatDetails(seatDetailId);

            // Assert
            Assert.That(result, Is.EqualTo(seatDetail));
        }

        [Test]
        public async Task GetByIdSeatDetails_NonExistingSeatDetailId_ReturnsNull()
        {
            // Arrange
            var nonExistingSeatDetailId = "B2";
            _mockSeatDetailRepository.Setup(repo => repo.GetAsync(nonExistingSeatDetailId)).ReturnsAsync((SeatDetail)null);

            // Act
            var result = await _seatDetailService.GetByIdSeatDetails(nonExistingSeatDetailId);

            // Assert
            Assert.IsNull(result);
        }
    }
}
