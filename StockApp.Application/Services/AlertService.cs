using System.Threading.Tasks;
using StockApp.Application.Interfaces.Services;

namespace StockApp.Application.Services
{
    public class AlertService : IAlertService
    {
        public async Task SendAlertAsync(string userId, string message)
        {
            await Task.CompletedTask;
        }
    }
}
