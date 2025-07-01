using Microsoft.ML;
using Moq;
using StockApp.Application.DTOs;
using StockApp.Application.Services;
using StockApp.Domain.Interfaces;
using System;
using System.Threading.Tasks;
using Xunit;

namespace StockApp.Domain.Test.Services
{
    public class SalesPredictionServiceTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly SalesPredictionService _predictionService;

        public SalesPredictionServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _predictionService = new SalesPredictionService(_productRepositoryMock.Object);
        }

        [Fact]
        public async Task PredictSalesAsync_ValidInput_ReturnsPrediction()
        {
            // Arrange
            var input = new SalesPredictionInputDTO
            {
                ProductId = 1,
                TargetDate = DateTime.Now.AddMonths(1),
                HistoricalMonths = 12
            };

            // Act
            var result = await _predictionService.PredictSalesAsync(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(input.ProductId, result.ProductId);
            Assert.Equal(input.TargetDate.Date, result.PredictionDate.Date);
            Assert.True(result.PredictedQuantity >= 0);
            Assert.True(result.Confidence >= 0 && result.Confidence <= 1);
            Assert.NotNull(result.ModelVersion);
        }

        [Fact]
        public async Task GetPredictionAccuracyAsync_ReturnsAccuracyMetric()
        {
            // Arrange
            int productId = 1;

            // Act
            var accuracy = await _predictionService.GetPredictionAccuracyAsync(productId);

            // Assert
            Assert.True(accuracy >= 0 && accuracy <= 1);
        }

        [Fact]
        public async Task GetSalesInsightsAsync_ReturnsValidInsights()
        {
            // Arrange
            int productId = 1;

            // Act
            var insights = await _predictionService.GetSalesInsightsAsync(productId);

            // Assert
            Assert.NotNull(insights);
            Assert.True(insights.ContainsKey("TrendGrowth"));
            Assert.True(insights.ContainsKey("SeasonalityIndex"));
            Assert.True(insights.ContainsKey("PredictionConfidence"));
        }

        [Theory]
        [InlineData(0)] // ID de produto inválido
        [InlineData(-1)] // ID de produto negativo
        public async Task PredictSalesAsync_InvalidProductId_ThrowsArgumentException(int invalidProductId)
        {
            // Arrange
            var input = new SalesPredictionInputDTO
            {
                ProductId = invalidProductId,
                TargetDate = DateTime.Now.AddMonths(1)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _predictionService.PredictSalesAsync(input));
        }

        [Fact]
        public async Task PredictSalesAsync_PastDate_ThrowsArgumentException()
        {
            // Arrange
            var input = new SalesPredictionInputDTO
            {
                ProductId = 1,
                TargetDate = DateTime.Now.AddDays(-1)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _predictionService.PredictSalesAsync(input));
        }
    }
}