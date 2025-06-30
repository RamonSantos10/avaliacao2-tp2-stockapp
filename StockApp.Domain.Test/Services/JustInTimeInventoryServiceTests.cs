using Xunit;
using Moq;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Application.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApp.Domain.Test
{
    public class JustInTimeInventoryServiceTests
    {
        [Fact]
        public async Task OptimizeInventoryAsync_ShouldReduceStock_WhenStockIsAboveTarget()
        {
            var mockProductRepository = new Mock<IProductRepository>();
            var products = new List<Product>
            {
                new Product(1, "ProductA", "DescA", 10.0m, 15, "imageA.png"),
                new Product(2, "ProductB", "DescB", 20.0m, 8, "imageB.png")
            };

            mockProductRepository.Setup(repo => repo.GetProducts())
                                 .ReturnsAsync(products);

            mockProductRepository.Setup(repo => repo.Update(It.IsAny<Product>()))
                                 .Returns(Task.CompletedTask);

            var service = new JustInTimeInventoryService(mockProductRepository.Object);

            await service.OptimizeInventoryAsync();

            mockProductRepository.Verify(repo => repo.GetProducts(), Times.Once);

            mockProductRepository.Verify(repo => repo.Update(It.Is<Product>(p => p.Id == 1 && p.Stock == 10)), Times.Once);

            mockProductRepository.Verify(repo => repo.Update(It.Is<Product>(p => p.Id == 2)), Times.Never);
        }

        [Fact]
        public async Task OptimizeInventoryAsync_ShouldIncreaseStock_WhenStockIsBelowHalfTarget()
        {
            var mockProductRepository = new Mock<IProductRepository>();
            var products = new List<Product>
            {
                new Product(1, "ProductA", "DescA", 10.0m, 3, "imageA.png"),
                new Product(2, "ProductB", "DescB", 20.0m, 7, "imageB.png")
            };

            mockProductRepository.Setup(repo => repo.GetProducts())
                                 .ReturnsAsync(products);

            mockProductRepository.Setup(repo => repo.Update(It.IsAny<Product>()))
                                 .Returns(Task.CompletedTask);

            var service = new JustInTimeInventoryService(mockProductRepository.Object);

            await service.OptimizeInventoryAsync();

            mockProductRepository.Verify(repo => repo.GetProducts(), Times.Once);

            mockProductRepository.Verify(repo => repo.Update(It.Is<Product>(p => p.Id == 1 && p.Stock == 10)), Times.Once);

            mockProductRepository.Verify(repo => repo.Update(It.Is<Product>(p => p.Id == 2)), Times.Never);
        }

        [Fact]
        public async Task OptimizeInventoryAsync_ShouldNotModifyStock_WhenStockIsWithinTargetRange()
        {
            var mockProductRepository = new Mock<IProductRepository>();
            var products = new List<Product>
            {
                new Product(1, "ProductA", "DescA", 10.0m, 7, "imageA.png"),
                new Product(2, "ProductB", "DescB", 20.0m, 10, "imageB.png")
            };

            mockProductRepository.Setup(repo => repo.GetProducts())
                                 .ReturnsAsync(products);

            var service = new JustInTimeInventoryService(mockProductRepository.Object);

            await service.OptimizeInventoryAsync();
            mockProductRepository.Verify(repo => repo.GetProducts(), Times.Once);
            mockProductRepository.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task OptimizeInventoryAsync_HandlesNoProductsGracefully()
        {
            var mockProductRepository = new Mock<IProductRepository>();

            mockProductRepository.Setup(repo => repo.GetProducts())
                                 .ReturnsAsync(new List<Product>());

            var service = new JustInTimeInventoryService(mockProductRepository.Object);

            await service.OptimizeInventoryAsync();

            mockProductRepository.Verify(repo => repo.GetProducts(), Times.Once);
            mockProductRepository.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Never);
        }
    }
}