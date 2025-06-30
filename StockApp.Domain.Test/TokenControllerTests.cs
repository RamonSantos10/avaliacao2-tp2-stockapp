using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockApp.API.Controllers;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using Xunit;

namespace StockApp.Domain.Test
{
    public class TokenControllerTests
    {
        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            var tokenController = new TokenController(authServiceMock.Object);

            authServiceMock.Setup(service => service.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .ReturnsAsync("valid-jwt-token");

            var userLoginDto = new UserLoginDto
            {
                Username = "testuser",
                Password = "password"
            };

            // Act
            var result = await tokenController.Login(userLoginDto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<TokenResponseDto>(result.Value);
            
            var tokenResponse = result.Value as TokenResponseDto;
            Assert.Equal("valid-jwt-token", tokenResponse.Token);
            Assert.True(tokenResponse.Expiration > DateTime.UtcNow);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            var tokenController = new TokenController(authServiceMock.Object);

            authServiceMock.Setup(service => service.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .ReturnsAsync((string)null);

            var userLoginDto = new UserLoginDto
            {
                Username = "invaliduser",
                Password = "wrongpassword"
            };

            // Act
            var result = await tokenController.Login(userLoginDto) as UnauthorizedObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
            Assert.Equal("Credenciais invalidas", result.Value);
        }

        [Fact]
        public async Task Login_AuthServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            var tokenController = new TokenController(authServiceMock.Object);

            authServiceMock.Setup(service => service.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .ThrowsAsync(new Exception("Database connection failed"));

            var userLoginDto = new UserLoginDto
            {
                Username = "testuser",
                Password = "password"
            };

            // Act
            var result = await tokenController.Login(userLoginDto) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Error during authentication", result.Value.ToString());
            Assert.Contains("Database connection failed", result.Value.ToString());
        }

        [Fact]
        public async Task Login_NullUserLoginDto_ReturnsBadRequest()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            var tokenController = new TokenController(authServiceMock.Object);

            // Act
            var result = await tokenController.Login(null) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Login não pode ser nulo", result.Value);
        }

        [Fact]
        public async Task Login_EmptyCredentials_ReturnsBadRequest()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            var tokenController = new TokenController(authServiceMock.Object);

            var userLoginDto = new UserLoginDto
            {
                Username = "",
                Password = ""
            };

            // Act
            var result = await tokenController.Login(userLoginDto) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Login não pode ser nulo", result.Value);
        }

        [Fact]
        public async Task Login_ValidCredentials_TokenExpirationIsSet24Hours()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            var tokenController = new TokenController(authServiceMock.Object);
            var beforeTest = DateTime.UtcNow;

            authServiceMock.Setup(service => service.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
                          .ReturnsAsync("valid-jwt-token");

            var userLoginDto = new UserLoginDto
            {
                Username = "testuser",
                Password = "password"
            };

            // Act
            var result = await tokenController.Login(userLoginDto) as OkObjectResult;
            var afterTest = DateTime.UtcNow;

            // Assert
            Assert.NotNull(result);
            var tokenResponse = result.Value as TokenResponseDto;
            
            // Check that expiration is approximately 24 hours from now (within 1 minute tolerance)
            var expectedExpiration = beforeTest.AddHours(24);
            var actualExpiration = tokenResponse.Expiration;
            
            Assert.True(actualExpiration >= expectedExpiration.AddMinutes(-1));
            Assert.True(actualExpiration <= afterTest.AddHours(24).AddMinutes(1));
        }

        [Fact]
        public async Task Login_NullUsername_ReturnsBadRequest()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            var tokenController = new TokenController(authServiceMock.Object);

            var userLoginDto = new UserLoginDto
            {
                Username = null,
                Password = "password"
            };

            // Act
            var result = await tokenController.Login(userLoginDto) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Login não pode ser nulo", result.Value);
        }

        [Fact]
        public async Task Login_NullPassword_ReturnsBadRequest()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();
            var tokenController = new TokenController(authServiceMock.Object);

            var userLoginDto = new UserLoginDto
            {
                Username = "testuser",
                Password = null
            };

            // Act
            var result = await tokenController.Login(userLoginDto) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Login não pode ser nulo", result.Value);
        }

        [Fact]
        public void TokenController_Constructor_InitializesCorrectly()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();

            // Act
            var tokenController = new TokenController(authServiceMock.Object);

            // Assert
            Assert.NotNull(tokenController);
        }

    }
}