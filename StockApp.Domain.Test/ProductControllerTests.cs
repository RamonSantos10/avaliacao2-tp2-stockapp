using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockApp.API.Controllers;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using Xunit;

namespace StockApp.Domain.Test
{
    public class ProductControllerTests
    {
        [Fact]
        public async Task ExportToCsv_ReturnsCsvFile_WithExpectedContent()
        {
            // Arrange
            var mockService = new Mock<IProductService>();
            var products = new List<ProductDTO>
            {
                new ProductDTO { Id = 1, Name = "Produto A", Description = "Descrição A", Price = 10.0m, Stock = 5 },
                new ProductDTO { Id = 2, Name = "Produto B", Description = "Descrição B", Price = 20.0m, Stock = 10 },
            };

            mockService.Setup(s => s.GetProducts()).ReturnsAsync(products);

            var controller = new ProductController(mockService.Object);

            // Act
            var result = await controller.ExportToCsv();

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);

            var content = Encoding.UTF8.GetString(fileResult.FileContents);
            Assert.Contains("Id,Name,Description,Price,Stock", content);
            Assert.Contains("1,\"Produto A\",\"Descrição A\",10.0,5", content);
            Assert.Contains("2,\"Produto B\",\"Descrição B\",20.0,10", content);

        }
    }
}
