using Microsoft.AspNetCore.Mvc;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using System.Threading.Tasks;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IWebhookService _webhookService;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(IWebhookService webhookService, ILogger<WebhookController> logger)
        {
            _webhookService = webhookService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint para receber webhooks de sistemas externos
        /// </summary>
        /// <param name="webhookDto">Dados do webhook</param>
        /// <returns>Resposta do processamento do webhook</returns>
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] WebhookDto webhookDto)
        {
            try
            {
                _logger.LogInformation("Received webhook for event: {Event}", webhookDto?.Event);

                if (webhookDto == null)
                {
                    _logger.LogWarning("Received null webhook data");
                    return BadRequest(new WebhookResponseDto
                    {
                        Success = false,
                        Message = "Webhook data is required"
                    });
                }

                var result = await _webhookService.ProcessWebhookAsync(webhookDto);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing webhook");
                return StatusCode(500, new WebhookResponseDto
                {
                    Success = false,
                    Message = "Internal server error processing webhook"
                });
            }
        }

        /// <summary>
        /// Endpoint para testar a funcionalidade de webhook
        /// </summary>
        /// <returns>Resposta de teste</returns>
        [HttpGet("test")]
        public async Task<IActionResult> TestWebhook()
        {
            try
            {
                var testData = new
                {
                    ProductId = 1,
                    ProductName = "Test Product",
                    Action = "Test"
                };

                await _webhookService.NotifyExternalSystemAsync("test.webhook", testData);

                return Ok(new WebhookResponseDto
                {
                    Success = true,
                    Message = "Test webhook executed successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing test webhook");
                return StatusCode(500, new WebhookResponseDto
                {
                    Success = false,
                    Message = "Error executing test webhook"
                });
            }
        }

        /// <summary>
        /// Endpoint para notificar sistemas externos sobre eventos de produtos
        /// </summary>
        /// <param name="eventType">Tipo do evento</param>
        /// <param name="data">Dados do evento</param>
        /// <returns>Resposta da notificação</returns>
        [HttpPost("notify")]
        public async Task<IActionResult> NotifyExternalSystems(
            [FromQuery] string eventType,
            [FromBody] object data)
        {
            try
            {
                if (string.IsNullOrEmpty(eventType))
                {
                    return BadRequest(new WebhookResponseDto
                    {
                        Success = false,
                        Message = "Event type is required"
                    });
                }

                await _webhookService.NotifyExternalSystemAsync(eventType, data);

                return Ok(new WebhookResponseDto
                {
                    Success = true,
                    Message = $"External systems notified about {eventType} event"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying external systems");
                return StatusCode(500, new WebhookResponseDto
                {
                    Success = false,
                    Message = "Error notifying external systems"
                });
            }
        }
    }
}