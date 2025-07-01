using StockApp.Domain.Interfaces;

namespace StockApp.Application.Services
{
     public class FakeSmsService : ISmsService
     {
        public Task SendSmsAsync(string phoneNumber, string message)
        {
                Console.WriteLine($"[FakeSmsService] Enviando SMS para {phoneNumber}: {message}");
                return Task.CompletedTask;
        }
     }   
}
