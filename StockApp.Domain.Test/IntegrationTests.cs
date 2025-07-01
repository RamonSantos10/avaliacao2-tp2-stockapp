using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;
using StockApp.API;
using StockApp.Application.DTOs;
using StockApp.Infra.Data.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace StockApp.Domain.Test
{
    public class IntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public IntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnSuccess()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "testuser",
                Password = "testpassword",
                Role = "User"
            };

            var json = JsonConvert.SerializeObject(registerDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/users/register", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("User registered successfully", responseString);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnToken()
        {
            // Arrange - First register a user
            var registerDto = new UserRegisterDto
            {
                Username = "loginuser",
                Password = "loginpassword",
                Role = "User"
            };

            var registerJson = JsonConvert.SerializeObject(registerDto);
            var registerContent = new StringContent(registerJson, Encoding.UTF8, "application/json");
            await _client.PostAsync("/api/users/register", registerContent);

            // Act - Now login
            var loginDto = new UserLoginDto
            {
                Username = "loginuser",
                Password = "loginpassword"
            };

            var loginJson = JsonConvert.SerializeObject(loginDto);
            var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/token/login", loginContent);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponseDto>(responseString);
            
            Assert.NotNull(tokenResponse);
            Assert.NotNull(tokenResponse.Token);
            Assert.Contains("token_loginuser", tokenResponse.Token);
        }

        [Fact]
        public async Task RegisterUser_WithDuplicateUsername_ShouldReturnBadRequest()
        {
            // Arrange - Register first user
            var registerDto = new UserRegisterDto
            {
                Username = "duplicateuser",
                Password = "password1",
                Role = "User"
            };

            var json = JsonConvert.SerializeObject(registerDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _client.PostAsync("/api/users/register", content);

            // Act - Try to register same username again
            var duplicateDto = new UserRegisterDto
            {
                Username = "duplicateuser",
                Password = "password2",
                Role = "Admin"
            };

            var duplicateJson = JsonConvert.SerializeObject(duplicateDto);
            var duplicateContent = new StringContent(duplicateJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/users/register", duplicateContent);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Username already exists", responseString);
        }

        [Fact]
        public async Task LoginUser_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginDto = new UserLoginDto
            {
                Username = "nonexistentuser",
                Password = "wrongpassword"
            };

            var json = JsonConvert.SerializeObject(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/token/login", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task RegisterUser_WithEmptyUsername_ShouldReturnBadRequest()
        {
            // Arrange
            var registerDto = new UserRegisterDto
            {
                Username = "",
                Password = "testpassword",
                Role = "User"
            };

            var json = JsonConvert.SerializeObject(registerDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/users/register", content);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove o DbContext existente
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Adiciona um banco em memória para testes
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
            });

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["ConnectionStrings:DefaultConnection"] = "InMemoryDatabase",
                    ["Jwt:Key"] = "minha-chave-secreta-super-segura-com-pelo-menos-32-caracteres",
                    ["Jwt:Issuer"] = "StockApp.API",
                    ["Jwt:Audience"] = "StockApp.Client"
                });
            });

            builder.UseEnvironment("Testing");
        }
    }
}