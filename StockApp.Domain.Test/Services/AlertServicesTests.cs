using System.Threading.Tasks;
using Xunit;
using StockApp.Application.Services;

namespace StockApp.Domain.Test.Services
{
    public class AlertServiceTests
    {
        [Fact]
        public async Task SendAlertAsync_Should_NotThrowException()
        {
            
            var service = new AlertService();

            var exception = await Record.ExceptionAsync(() => service.SendAlertAsync("user123", "Mensagem de teste"));

            Assert.Null(exception);
        }
    }
}
