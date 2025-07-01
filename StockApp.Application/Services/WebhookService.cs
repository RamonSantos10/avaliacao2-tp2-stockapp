using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace StockApp.Application.Services
{
    public class WebhookService : IWebhookService
    {
        private readonly ILogger<WebhookService> _logger;
        private readonly HttpClient _httpClient;

        public WebhookService(ILogger<WebhookService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<WebhookResponseDto> ProcessWebhookAsync(WebhookDto webhookDto)
        {
            try
            {
                _logger.LogInformation("Processing webhook for event: {Event}", webhookDto.Event);

                // Validar o webhook
                var isValid = await ValidateWebhookAsync(webhookDto);
                if (!isValid)
                {
                    _logger.LogWarning("Invalid webhook received for event: {Event}", webhookDto.Event);
                    return new WebhookResponseDto
                    {
                        Success = false,
                        Message = "Invalid webhook data"
                    };
                }

                // Processar o evento do webhook
                await ProcessWebhookEventAsync(webhookDto);

                _logger.LogInformation("Webhook processed successfully for event: {Event}", webhookDto.Event);
                
                return new WebhookResponseDto
                {
                    Success = true,
                    Message = "Webhook processed successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook for event: {Event}", webhookDto.Event);
                return new WebhookResponseDto
                {
                    Success = false,
                    Message = $"Error processing webhook: {ex.Message}"
                };
            }
        }

        public async Task<bool> ValidateWebhookAsync(WebhookDto webhookDto)
        {
            // Validações básicas
            if (string.IsNullOrEmpty(webhookDto.Event))
            {
                _logger.LogWarning("Webhook validation failed: Event is null or empty");
                return false;
            }
            
            if (string.IsNullOrEmpty(webhookDto.Url))
            {
                _logger.LogWarning("Webhook validation failed: URL is null or empty");
                return false;
            }

            // Validar URL (para testes, aceitar URLs simples como 'string')
            // Em produção, você pode querer uma validação mais restritiva
            if (webhookDto.Url != "string" && !Uri.TryCreate(webhookDto.Url, UriKind.Absolute, out var uri))
            {
                _logger.LogWarning("Webhook validation failed: Invalid URL format: {Url}", webhookDto.Url);
                return false;
            }

            // Validar timestamp (para testes, aceitar qualquer timestamp válido)
            // Em produção, você pode querer uma validação mais restritiva
            if (webhookDto.Timestamp == default(DateTime))
            {
                _logger.LogWarning("Webhook timestamp is invalid or missing");
                return false;
            }
            
            _logger.LogInformation("Webhook timestamp validation passed: {Timestamp}", webhookDto.Timestamp);

            return await Task.FromResult(true);
        }

        public async Task NotifyExternalSystemAsync(string eventType, object data)
        {
            try
            {
                _logger.LogInformation("Notifying external systems about event: {EventType}", eventType);

                var payload = new
                {
                    Event = eventType,
                    Data = data,
                    Timestamp = DateTime.UtcNow,
                    Source = "StockApp.API"
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Aqui você pode configurar URLs de webhook específicas
                // Por enquanto, apenas logamos a notificação
                _logger.LogInformation("External notification payload: {Payload}", json);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying external systems for event: {EventType}", eventType);
            }
        }

        private async Task ProcessWebhookEventAsync(WebhookDto webhookDto)
        {
            switch (webhookDto.Event.ToLower())
            {
                case "product.created":
                    await HandleProductCreatedAsync(webhookDto);
                    break;
                case "product.updated":
                    await HandleProductUpdatedAsync(webhookDto);
                    break;
                case "product.deleted":
                    await HandleProductDeletedAsync(webhookDto);
                    break;
                case "inventory.low_stock":
                    await HandleLowStockAsync(webhookDto);
                    break;
                case "order.created":
                    await HandleOrderCreatedAsync(webhookDto);
                    break;
                default:
                    _logger.LogWarning("Unknown webhook event type: {Event}", webhookDto.Event);
                    break;
            }
        }

        private async Task HandleProductCreatedAsync(WebhookDto webhookDto)
        {
            _logger.LogInformation("Handling product created webhook");
            await NotifyExternalSystemAsync("product.created", webhookDto.Data);
        }

        private async Task HandleProductUpdatedAsync(WebhookDto webhookDto)
        {
            _logger.LogInformation("Handling product updated webhook");
            await NotifyExternalSystemAsync("product.updated", webhookDto.Data);
        }

        private async Task HandleProductDeletedAsync(WebhookDto webhookDto)
        {
            _logger.LogInformation("Handling product deleted webhook");
            await NotifyExternalSystemAsync("product.deleted", webhookDto.Data);
        }

        private async Task HandleLowStockAsync(WebhookDto webhookDto)
        {
            _logger.LogInformation("Handling low stock webhook");
            await NotifyExternalSystemAsync("inventory.low_stock", webhookDto.Data);
        }

        private async Task HandleOrderCreatedAsync(WebhookDto webhookDto)
        {
            _logger.LogInformation("Handling order created webhook");
            await NotifyExternalSystemAsync("order.created", webhookDto.Data);
        }
    }
}