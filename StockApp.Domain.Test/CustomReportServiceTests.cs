using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StockApp.Application.Services;
using StockApp.Application.DTOs;
using StockApp.Domain.Interfaces;
using StockApp.Domain.Entities;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace StockApp.Domain.Test
{
    [TestClass]
    public class CustomReportServiceTests
    {
        private Mock<IProductRepository> _mockProductRepository;
        private Mock<ICategoryRepository> _mockCategoryRepository;
        private Mock<IMapper> _mockMapper;
        private CustomReportService _customReportService;

        [TestInitialize]
        public void Setup()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockMapper = new Mock<IMapper>();
            _customReportService = new CustomReportService(
                _mockProductRepository.Object,
                _mockCategoryRepository.Object,
                _mockMapper.Object
            );
        }

        [TestMethod]
        public async Task GenerateReportAsync_SalesReport_ReturnsValidReport()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product(1, "Produto 1", "Descrição do Produto 1", 100, 10, "image1.jpg") { CategoryId = 1 },
                new Product(2, "Produto 2", "Descrição do Produto 2", 200, 5, "image2.jpg") { CategoryId = 1 }
            };

            _mockProductRepository.Setup(x => x.GetProducts())
                .ReturnsAsync(products);

            var parameters = new ReportParametersDto
            {
                ReportType = "sales",
                IncludeDetails = true
            };

            // Act
            var result = await _customReportService.GenerateReportAsync(parameters);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Relatório de Vendas", result.Title);
            Assert.AreEqual("Sales", result.ReportType);
            Assert.IsTrue(result.Data.Count > 0);
            Assert.IsNotNull(result.Summary);
        }

        [TestMethod]
        public async Task GenerateInventoryReportAsync_ReturnsValidInventoryReport()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product(1, "Produto 1", "Descrição do Produto 1", 100, 5, "image1.jpg") { CategoryId = 1 },
                new Product(2, "Produto 2", "Descrição do Produto 2", 200, 15, "image2.jpg") { CategoryId = 1 }
            };

            _mockProductRepository.Setup(x => x.GetProducts())
                .ReturnsAsync(products);

            var parameters = new ReportParametersDto
            {
                ReportType = "inventory"
            };

            // Act
            var result = await _customReportService.GenerateInventoryReportAsync(parameters);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Relatório de Inventário", result.Title);
            Assert.AreEqual("Inventory", result.ReportType);
            Assert.AreEqual(2, result.Data.Count);
            Assert.IsTrue(result.Summary.TotalValue > 0);
            
            // Verifica se produtos com estoque baixo são identificados
            var lowStockItems = (int)result.Summary.AdditionalMetrics["LowStockItems"];
            Assert.AreEqual(1, lowStockItems); // Produto 1 tem estoque < 10
        }

        [TestMethod]
        public async Task GenerateCategoryReportAsync_ReturnsValidCategoryReport()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product(1, "Produto 1", "Descrição do Produto 1", 100, 10, "image1.jpg") { CategoryId = 1 },
                new Product(2, "Produto 2", "Descrição do Produto 2", 200, 5, "image2.jpg") { CategoryId = 2 }
            };

            var categories = new List<Category>
            {
                new Category(1, "Categoria 1"),
                new Category(2, "Categoria 2")
            };

            _mockProductRepository.Setup(x => x.GetProducts())
                .ReturnsAsync(products);
            _mockCategoryRepository.Setup(x => x.GetCategories())
                .ReturnsAsync(categories);

            var parameters = new ReportParametersDto
            {
                ReportType = "category"
            };

            // Act
            var result = await _customReportService.GenerateCategoryReportAsync(parameters);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Relatório por Categoria", result.Title);
            Assert.AreEqual("Category", result.ReportType);
            Assert.AreEqual(2, result.Data.Count);
            Assert.IsTrue(result.Summary.TotalValue > 0);
        }

        [TestMethod]
        public async Task GetAvailableReportTypesAsync_ReturnsExpectedTypes()
        {
            // Act
            var result = await _customReportService.GetAvailableReportTypesAsync();

            // Assert
            Assert.IsNotNull(result);
            var types = result.ToList();
            Assert.IsTrue(types.Contains("Sales"));
            Assert.IsTrue(types.Contains("Inventory"));
            Assert.IsTrue(types.Contains("Performance"));
            Assert.IsTrue(types.Contains("Category"));
        }

        [TestMethod]
        public async Task GenerateReportAsync_DefaultReport_ReturnsValidReport()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product(1, "Produto 1", "Descrição do Produto 1", 100, 10, "image1.jpg") { CategoryId = 1 }
            };

            _mockProductRepository.Setup(x => x.GetProducts())
                .ReturnsAsync(products);

            var parameters = new ReportParametersDto
            {
                ReportType = "unknown"
            };

            // Act
            var result = await _customReportService.GenerateReportAsync(parameters);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Relatório Personalizado", result.Title);
            Assert.AreEqual("Default", result.ReportType);
            Assert.AreEqual(2, result.Data.Count); // TotalProdutos e ValorTotalEstoque
        }
    }
}