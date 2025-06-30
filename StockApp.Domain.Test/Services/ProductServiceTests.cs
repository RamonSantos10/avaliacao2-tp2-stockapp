using Xunit;
using Moq;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Application.DTOs;
using StockApp.Application.Services;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockApp.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;


namespace StockApp.Domain.Test
{
    public class ProductServiceTests
    {
        [Fact]
        public async Task GetProductsByIdsAsync_ShouldReturnMappedProductsWhenFound()
        {
            var mockRepo = new Mock<IProductRepository>();
            var mockMapper = new Mock<IMapper>();

            var fakeProductEntities = new List<Product>
            {
                new Product(1, "ProductA", "DescA", 10.0m, 100, "imageA.png"),
                new Product(2, "ProductB", "DescB", 20.0m, 50, "imageB.png")
            };

            mockRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>()))
                    .ReturnsAsync(fakeProductEntities);

            mockMapper.Setup(m => m.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
                      .Returns((IEnumerable<Product> source) =>
                          source.Select(p => new ProductDTO { Id = p.Id, Name = p.Name, Price = p.Price }));

            var service = new ProductService(mockRepo.Object, mockMapper.Object);

            var productIds = new List<int> { 1, 2 };

            var result = await service.GetProductsByIdsAsync(productIds);

            mockRepo.Verify(r => r.GetByIdsAsync(productIds), Times.Once);
            mockMapper.Verify(m => m.Map<IEnumerable<ProductDTO>>(fakeProductEntities), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.Id == 1);
            Assert.Contains(result, p => p.Id == 2);
        }

        [Fact]
        public async Task GetProductsByIdsAsync_ShouldReturnEmptyWhenNoProductsFound()
        {
            var mockRepo = new Mock<IProductRepository>();
            var mockMapper = new Mock<IMapper>();

            mockRepo.Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>()))
                    .ReturnsAsync(new List<Product>());

            mockMapper.Setup(m => m.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
                      .Returns(Enumerable.Empty<ProductDTO>());

            var service = new ProductService(mockRepo.Object, mockMapper.Object);
            var productIds = new List<int> { 99, 100 };

            var result = await service.GetProductsByIdsAsync(productIds);

            mockRepo.Verify(r => r.GetByIdsAsync(productIds), Times.Once);
            mockMapper.Verify(m => m.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()), Times.Once);
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProductsByIdsAsync_ShouldReturnEmptyWhenEmptyIdListProvided()
        {
            var mockRepo = new Mock<IProductRepository>();
            var mockMapper = new Mock<IMapper>();

            mockRepo.Setup(r => r.GetByIdsAsync(It.Is<List<int>>(l => !l.Any())))
                    .ReturnsAsync(new List<Product>());

            mockMapper.Setup(m => m.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
                      .Returns(Enumerable.Empty<ProductDTO>());

            var service = new ProductService(mockRepo.Object, mockMapper.Object);
            var productIds = new List<int>();

            var result = await service.GetProductsByIdsAsync(productIds);

            mockRepo.Verify(r => r.GetByIdsAsync(productIds), Times.Once);
            mockMapper.Verify(m => m.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()), Times.Once);
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        [Fact]
        public async Task GetLowStockAsync_Returns_Products_With_Stock_Less_Or_Equal_Threshold()
        {
            var threshold = 10;
            var products = new List<Product>
            {
                new Product(1, "Produto1", "descrição", 10.0m, 5, "img"),
                new Product(2, "Produto2", "descrição", 20.0m, 10, "img"),
                new Product(3, "Produto3", "descrição", 30.0m, 15, "img") 
            };

            var mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(r => r.GetLowStockAsync(threshold))
                .ReturnsAsync(products.Where(p => p.Stock <= threshold).ToList());

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
                .Returns((IEnumerable<Product> source) =>
                    source.Select(p => new ProductDTO { Id = p.Id, Name = p.Name, Price = p.Price }));

            var service = new ProductService(mockRepo.Object, mockMapper.Object);

            var result = await service.GetLowStockAsync(threshold);

            Assert.NotNull(result);
            Assert.All(result, p => Assert.True(p.Stock <= threshold));
            Assert.Equal(2, result.Count());
        }
    }
}
