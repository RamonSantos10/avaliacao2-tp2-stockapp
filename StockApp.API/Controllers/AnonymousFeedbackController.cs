using Microsoft.AspNetCore.Mvc;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnonymousFeedbackController : ControllerBase
    {
        private readonly IAnonymousFeedbackService _anonymousFeedbackService;

        public AnonymousFeedbackController(IAnonymousFeedbackService anonymousFeedbackService)
        {
            _anonymousFeedbackService = anonymousFeedbackService;
        }

        /// <summary>
        /// Coleta feedback anônimo de clientes
        /// </summary>
        /// <param name="feedbackDto">Dados do feedback anônimo</param>
        /// <returns>Confirmação de recebimento com análise de sentimento</returns>
        [HttpPost("collect")]
        public async Task<IActionResult> CollectFeedback([FromBody] AnonymousFeedbackDTO feedbackDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Capturar informações do contexto HTTP com melhor detecção de IP
                var ipAddress = GetClientIpAddress();
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                
                await _anonymousFeedbackService.CollectFeedbackAsync(
                    feedbackDto.Feedback,
                    ipAddress,
                    userAgent
                );
                
                var response = new AnonymousFeedbackResponseDTO
                {
                    Success = true,
                    Message = "Feedback anônimo coletado com sucesso!",
                    Timestamp = DateTime.UtcNow,
                    SentimentAnalysis = "Análise de sentimento processada"
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint alternativo para coleta de feedback via query parameter
        /// </summary>
        /// <param name="message">Texto do feedback</param>
        /// <returns>Confirmação de recebimento</returns>
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitFeedback([FromQuery] string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("Mensagem de feedback é obrigatória.");
            }

            try
            {
                // Capturar informações do contexto HTTP com melhor detecção de IP
                var ipAddress = GetClientIpAddress();
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                
                await _anonymousFeedbackService.CollectFeedbackAsync(message, ipAddress, userAgent);
                return Ok(new { message = "Feedback anônimo enviado com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllFeedbacks()
        {
            try
            {
                var feedbacks = await _anonymousFeedbackService.GetAllFeedbacksAsync();
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpGet("sentiment/{sentiment}")]
        public async Task<IActionResult> GetFeedbacksBySentiment(string sentiment)
        {
            try
            {
                var feedbacks = await _anonymousFeedbackService.GetFeedbacksBySentimentAsync(sentiment);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Método para capturar o IP real do cliente, considerando proxies e load balancers
        /// </summary>
        /// <returns>Endereço IP do cliente</returns>
        private string GetClientIpAddress()
        {
            // Verificar headers de proxy primeiro
            var xForwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xForwardedFor))
            {
                // X-Forwarded-For pode conter múltiplos IPs separados por vírgula
                // O primeiro é geralmente o IP original do cliente
                return xForwardedFor.Split(',')[0].Trim();
            }

            var xRealIp = HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xRealIp))
            {
                return xRealIp;
            }

            // Fallback para o IP da conexão direta
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            
            // Converter IPv6 loopback para IPv4 se necessário
            if (remoteIp == "::1")
            {
                return "127.0.0.1";
            }

            return remoteIp ?? "Unknown";
        }
    }
}