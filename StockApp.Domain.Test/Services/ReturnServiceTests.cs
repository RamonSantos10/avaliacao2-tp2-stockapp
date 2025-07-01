using Xunit;
using Moq;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Application.Services;
using StockApp.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using StockApp.Application.DTOs;
using AutoMapper;

namespace StockApp.Domain.Test.Services
{
    public class ReturnServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ReturnService _returnService;

        public ReturnServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _productRepositoryMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>();
            _returnService = new ReturnService(_productRepositoryMock.Object, _context, _mapperMock.Object);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task ProcessReturn_ValidReturn_ShouldUpdateStockAndCreateReturn()
        {
            // Arrange
            var returnDto = new ReturnProductDTO
            {
                OrderId = 1,
                ProductId = 1,
                Quantity = 2,
                ReturnReason = "Produto com defeito de fabricação"
            };

            var product = new Product { Id = 1, Name = "Test Product", Stock = 5 };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(returnDto.ProductId))
                .ReturnsAsync(product);

            _productRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _returnService.ProcessReturnAsync(returnDto);

            // Assert
            Assert.True(result);
            Assert.Equal(7, product.Stock); // Verifica se o estoque foi atualizado corretamente
            _productRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Product>(p => p.Stock == 7)), Times.Once);
        }

        [Fact]
        public async Task ProcessReturn_InvalidProductId_ShouldReturnFalse()
        {
            // Arrange
            var returnDto = new ReturnProductDTO
            {
                OrderId = 1,
                ProductId = 999,
                Quantity = 1,
                ReturnReason = "Produto com defeito"
            };

            _productRepositoryMock.Setup(repo => repo.GetByIdAsync(returnDto.ProductId))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _returnService.ProcessReturnAsync(returnDto);

            // Assert
            Assert.False(result);
        }
    }
}