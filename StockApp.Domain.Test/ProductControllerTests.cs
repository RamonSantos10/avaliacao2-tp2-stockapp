using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockApp.API.Controllers;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using Xunit;
using System.Linq; 

namespace StockApp.Domain.Test
{
    public class ProductControllerTests
    {
        [Fact]
        public async Task ExportToCsv_ReturnsCsvFile_WithExpectedContent()
        {
            var mockService = new Mock<IProductService>();
            var products = new List<ProductDTO>
            {
                new ProductDTO { Id = 1, Name = "Produto A", Description = "Descrição A", Price = 10.0m, Stock = 5 },
                new ProductDTO { Id = 2, Name = "Produto B", Description = "Descrição B", Price = 20.0m, Stock = 10 },
            };

            mockService.Setup(s => s.GetProducts()).ReturnsAsync(products);

            var controller = new ProductController(mockService.Object);

            var result = await controller.ExportToCsv();

            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);

            var content = Encoding.UTF8.GetString(fileResult.FileContents);
            Assert.Contains("Id,Name,Description,Price,Stock", content);
            Assert.Contains("1,\"Produto A\",\"Descrição A\",10.0,5", content);
            Assert.Contains("2,\"Produto B\",\"Descrição B\",20.0,10", content);
        }

        [Fact]
        public async Task CompareProducts_ReturnsOk_WithProducts()
        {
            var mockProductService = new Mock<IProductService>();
            var productIds = new List<int> { 1, 2 };
            var productsDto = new List<ProductDTO>
            {
                new ProductDTO { Id = 1, Name = "Product A", Price = 10.0m, Stock = 100 },
                new ProductDTO { Id = 2, Name = "Product B", Price = 20.0m, Stock = 50 }
            };

            mockProductService.Setup(s => s.GetProductsByIdsAsync(productIds))
                              .ReturnsAsync(productsDto);

            var controller = new ProductController(mockProductService.Object);

            var result = await controller.CompareProducts(productIds);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductDTO>>(okResult.Value);

            Assert.Equal(2, returnedProducts.Count());
            Assert.Contains(returnedProducts, p => p.Id == 1);
            Assert.Contains(returnedProducts, p => p.Id == 2);
            mockProductService.Verify(s => s.GetProductsByIdsAsync(productIds), Times.Once);
        }

        [Fact]
        public async Task CompareProducts_ReturnsBadRequest_WhenProductIdsAreNull()
        {
            var mockProductService = new Mock<IProductService>();
            var controller = new ProductController(mockProductService.Object);
            List<int> productIds = null;

            var result = await controller.CompareProducts(productIds);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("A lista de IDs de produtos não pode estar vazia.", badRequestResult.Value);
            mockProductService.Verify(s => s.GetProductsByIdsAsync(It.IsAny<List<int>>()), Times.Never);
        }

        [Fact]
        public async Task CompareProducts_ReturnsBadRequest_WhenProductIdsAreEmpty()
        {
            var mockProductService = new Mock<IProductService>();
            var controller = new ProductController(mockProductService.Object);
            var productIds = new List<int>();

            var result = await controller.CompareProducts(productIds);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("A lista de IDs de produtos não pode estar vazia.", badRequestResult.Value);
            mockProductService.Verify(s => s.GetProductsByIdsAsync(It.IsAny<List<int>>()), Times.Never);
        }

        [Fact]
        public async Task CompareProducts_ReturnsNotFound_WhenNoProductsFoundByService()
        {
            var mockProductService = new Mock<IProductService>();
            var productIds = new List<int> { 99, 100 };

            mockProductService.Setup(s => s.GetProductsByIdsAsync(productIds))
                              .ReturnsAsync(Enumerable.Empty<ProductDTO>());

            var controller = new ProductController(mockProductService.Object);

            var result = await controller.CompareProducts(productIds);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Nenhum produto encontrado para os IDs fornecidos.", notFoundResult.Value);
            mockProductService.Verify(s => s.GetProductsByIdsAsync(productIds), Times.Once);
        }

        [Fact]
        public async Task CompareProducts_ReturnsNotFound_WhenServiceReturnsNull()
        {
            var mockProductService = new Mock<IProductService>();
            var productIds = new List<int> { 99, 100 };

            mockProductService.Setup(s => s.GetProductsByIdsAsync(productIds))
                              .ReturnsAsync((IEnumerable<ProductDTO>)null);

            var controller = new ProductController(mockProductService.Object);

            var result = await controller.CompareProducts(productIds);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Nenhum produto encontrado para os IDs fornecidos.", notFoundResult.Value);
            mockProductService.Verify(s => s.GetProductsByIdsAsync(productIds), Times.Once);
        }
    }
}