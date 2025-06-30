using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockApp.Application.Services;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace StockApp.Domain.Test.Services
{
    [TestClass]
    public class InventoryServiceTests
    {
        private Mock<IProductRepository> _mockProductRepository;
        private InventoryService _inventoryService;

        [TestInitialize]
        public void Setup()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _inventoryService = new InventoryService(_mockProductRepository.Object);
        }

        [TestMethod]
        public async Task ReplenishStockAsync_ShouldReplenishLowStockProducts_WithDefaultValues()
        {
            // Arrange
            var lowStockProducts = new List<Product>
            {
                new Product(1, "Product 1", "Description 1", 10.0m, 5, "image1.jpg") { CategoryId = 1 },
                new Product(2, "Product 2", "Description 2", 20.0m, 8, "image2.jpg") { CategoryId = 1 }
            };

            _mockProductRepository.Setup(x => x.GetLowStockAsync(10))
                .ReturnsAsync(lowStockProducts);

            _mockProductRepository.Setup(x => x.Update(It.IsAny<Product>()))
                .ReturnsAsync((Product p) => p);

            // Act
            await _inventoryService.ReplenishStockAsync();

            // Assert
            Assert.AreEqual(55, lowStockProducts[0].Stock); // 5 + 50
            Assert.AreEqual(58, lowStockProducts[1].Stock); // 8 + 50
            _mockProductRepository.Verify(x => x.GetLowStockAsync(10), Times.Once);
            _mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task ReplenishStockAsync_ShouldReplenishLowStockProducts_WithCustomValues()
        {
            // Arrange
            var threshold = 15;
            var replenishQuantity = 25;
            var lowStockProducts = new List<Product>
            {
                new Product(1, "Product 1", "Description 1", 10.0m, 10, "image1.jpg") { CategoryId = 1 }
            };

            _mockProductRepository.Setup(x => x.GetLowStockAsync(threshold))
                .ReturnsAsync(lowStockProducts);

            _mockProductRepository.Setup(x => x.Update(It.IsAny<Product>()))
                .ReturnsAsync((Product p) => p);

            // Act
            await _inventoryService.ReplenishStockAsync(threshold, replenishQuantity);

            // Assert
            Assert.AreEqual(35, lowStockProducts[0].Stock); // 10 + 25
            _mockProductRepository.Verify(x => x.GetLowStockAsync(threshold), Times.Once);
            _mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
        }

        [TestMethod]
        public async Task ReplenishStockAsync_ShouldNotUpdateProducts_WhenNoLowStockProducts()
        {
            // Arrange
            var emptyProductList = new List<Product>();

            _mockProductRepository.Setup(x => x.GetLowStockAsync(10))
                .ReturnsAsync(emptyProductList);

            // Act
            await _inventoryService.ReplenishStockAsync();

            // Assert
            _mockProductRepository.Verify(x => x.GetLowStockAsync(10), Times.Once);
            _mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Never);
        }
    }
}