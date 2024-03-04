using Microsoft.Extensions.Configuration;
using Moq;
using Simplifly.Interfaces;
using Simplifly.Models.DTOs;
using Simplifly.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpliflyTest
{
    internal class TokenServiceTest
    {
        [Test]
        public async Task GenerateToken()
        {
            // Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(e => e["SecretKey"]).Returns("This is a Dummy key created for authentication purpose");
            ITokenService tokenService = new TokenService(mockConfiguration.Object);

            var user = new LoginUserDTO
            {
                Username = "Abhinav",
                Role = "customer"
            };

            // Act
            var token = await tokenService.GenerateToken(user);

            // Assert
            Assert.IsNotNull(token);
        }
    }
}
