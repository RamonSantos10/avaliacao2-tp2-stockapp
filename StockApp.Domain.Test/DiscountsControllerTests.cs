using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using StockApp.API.Controllers;
using StockApp.Application.Interfaces;
using System.Threading.Tasks;

namespace StockApp.Domain.Test
{
    public class DiscountsControllerTests
    {
        [Fact]
        public void ApplyDiscount_ReturnsOkResult_WithCorrectCalculatedPrice()
        {
            var mockDiscountService = new Mock<IDiscountService>();
            decimal initialPrice = 100.0m;
            decimal discount = 10.0m;
            decimal expectedFinalPrice = 90.0m;

            mockDiscountService.Setup(s => s.ApplyDiscount(initialPrice, discount))
                               .Returns(expectedFinalPrice);

            var controller = new DiscountsController(mockDiscountService.Object);

            var result = controller.ApplyDiscount(initialPrice, discount);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedFinalPrice, okResult.Value);

            mockDiscountService.Verify(s => s.ApplyDiscount(initialPrice, discount), Times.Once);
        }

        [Theory]
        [InlineData(50.0, 20.0, 40.0)]
        [InlineData(200.0, 50.0, 100.0)]
        public void ApplyDiscount_ReturnsOkResult_WithVariousInputs(decimal price, decimal percentage, decimal expected)
        {
            var mockDiscountService = new Mock<IDiscountService>();
            mockDiscountService.Setup(s => s.ApplyDiscount(price, percentage))
                               .Returns(expected);

            var controller = new DiscountsController(mockDiscountService.Object);

            var result = controller.ApplyDiscount(price, percentage);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
            mockDiscountService.Verify(s => s.ApplyDiscount(price, percentage), Times.Once);
        }

        [Theory]
        [InlineData(100.0, -10.0, 110.0)]
        [InlineData(50.0, 110.0, -5.0)]
        public void ApplyDiscount_ReturnsOkResult_ForEdgeCasesHandledByService(decimal price, decimal percentage, decimal expected)
        {
            var mockDiscountService = new Mock<IDiscountService>();
            mockDiscountService.Setup(s => s.ApplyDiscount(price, percentage))
                               .Returns(expected);

            var controller = new DiscountsController(mockDiscountService.Object);

            var result = controller.ApplyDiscount(price, percentage);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, okResult.Value);
            mockDiscountService.Verify(s => s.ApplyDiscount(price, percentage), Times.Once);
        }
    }
}