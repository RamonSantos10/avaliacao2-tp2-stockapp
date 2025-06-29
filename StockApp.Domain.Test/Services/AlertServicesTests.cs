
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using StockApp.Application.Services;

namespace StockApp.Domain.Test.Services
{
    public class AlertServiceTests
    {
        [Fact]
        public async Task SendAlertAsync_Should_NotThrowException()
        {
            
            var mockLogger = new Mock<ILogger<AlertService>>();

            var service = new AlertService(mockLogger.Object);

            var exception = await Record.ExceptionAsync(() => service.SendAlertAsync("user123", "Mensagem de teste"));

            Assert.Null(exception);

        }
    }
}