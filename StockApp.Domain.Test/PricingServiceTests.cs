using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using StockApp.Application.Services;
using StockApp.Application.DTOs;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StockApp.Domain.Test
{
    [TestClass]
    public class PricingServiceTests
    {
        [TestMethod]
        public async Task GetProductDetailsAsync_ShouldReturnProductDetails_WhenApiReturnsValidResponse()
        {
            // Arrange
            var productId = "1";
            var responseContent = @"{
                ""id"": 1,
                ""title"": ""Essence Mascara Lash Princess"",
                ""description"": ""The Essence Mascara Lash Princess is a popular mascara known for its volumizing and lengthening effects."",
                ""price"": 9.99,
                ""stock"": 5,
                ""category"": ""beauty"",
                ""brand"": ""Essence""
            }";

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new System.Uri("https://dummyjson.com/")
            };

            var pricingService = new PricingService(httpClient);

            // Act
            var result = await pricingService.GetProductDetailsAsync(productId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("1", result.Id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Essence Mascara Lash Princess", result.Name);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("The Essence Mascara Lash Princess is a popular mascara known for its volumizing and lengthening effects.", result.Description);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(9.99m, result.Price);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(5, result.Stock);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("beauty", result.Category);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Essence", result.Brand);
        }

        [TestMethod]
        public async Task GetProductDetailsAsync_ShouldReturnDefaultDetails_WhenApiReturnsError()
        {
            // Arrange
            var productId = "999";

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new System.Uri("https://dummyjson.com/")
            };

            var pricingService = new PricingService(httpClient);

            // Act
            var result = await pricingService.GetProductDetailsAsync(productId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("999", result.Id);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Produto Não Encontrado", result.Name);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Descrição não disponível", result.Description);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(99.99m, result.Price);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, result.Stock);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Geral", result.Category);
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("N/A", result.Brand);
        }
    }
}