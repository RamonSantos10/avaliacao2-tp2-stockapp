using Xunit;
using StockApp.Application.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApp.Domain.Test
{
    public class DiscountServiceTests
    {
        [Theory]
        [InlineData(100.0, 10.0, 90.0)]
        [InlineData(50.0, 20.0, 40.0)]
        [InlineData(75.50, 5.0, 71.725)]
        [InlineData(100.0, 0.0, 100.0)]
        [InlineData(200.0, 100.0, 0.0)]
        [InlineData(0.0, 10.0, 0.0)]
        public void ApplyDiscount_ShouldCalculateCorrectly(decimal price, decimal discountPercentage, decimal expectedResult)
        {
            var service = new DiscountService();
            var result = service.ApplyDiscount(price, discountPercentage);
            Assert.Equal(expectedResult, result, 3);
        }

        [Theory]
        [InlineData(100.0, -10.0)]
        [InlineData(50.0, 110.0)]
        public void ApplyDiscount_ShouldHandleEdgeCases(decimal price, decimal discountPercentage)
        {
            var service = new DiscountService();
            var result = service.ApplyDiscount(price, discountPercentage);

            if (discountPercentage < 0)
            {
                Assert.True(result > price);
            }
            else if (discountPercentage > 100)
            {
                Assert.True(result < 0);
            }
        }
    }
}