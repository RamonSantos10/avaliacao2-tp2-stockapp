using System.Threading.Tasks;
using StockApp.Application.Interfaces.Services; 
using Microsoft.Extensions.Logging; 

namespace StockApp.Application.Services
{
    public class AlertService : IAlertService
    {
        private readonly ILogger<AlertService> _logger; 

        public AlertService(ILogger<AlertService> logger) 
        {
            _logger = logger;
        }

        public async Task SendAlertAsync(string userId, string message)
        {
           
            _logger.LogInformation($"Alerta enviado para o usuário {userId}: {message}");
            await Task.CompletedTask; 
        }
    }
}