using StockApp.Application.DTOs;
using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    public interface IWebhookService
    {
        Task<WebhookResponseDto> ProcessWebhookAsync(WebhookDto webhookDto);
        Task<bool> ValidateWebhookAsync(WebhookDto webhookDto);
        Task NotifyExternalSystemAsync(string eventType, object data);
    }
}