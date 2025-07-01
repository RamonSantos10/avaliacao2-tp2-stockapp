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
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _anonymousFeedbackService.CollectFeedbackAsync(feedbackDto.Feedback);
                
                var response = new AnonymousFeedbackResponseDTO
                {
                    Success = true,
                    Message = "Feedback anônimo coletado com sucesso!",
                    Timestamp = DateTime.UtcNow,
                    SentimentAnalysis = "Análise realizada"
                };
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorResponse = new AnonymousFeedbackResponseDTO
                {
                    Success = false,
                    Message = "Erro ao processar feedback",
                    Timestamp = DateTime.UtcNow
                };
                
                return StatusCode(500, errorResponse);
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
            try
            {
                if (string.IsNullOrWhiteSpace(message))
                {
                    return BadRequest("Mensagem de feedback não pode estar vazia.");
                }

                await _anonymousFeedbackService.CollectFeedbackAsync(message);
                
                return Ok(new { 
                    success = true,
                    message = "Obrigado pelo seu feedback anônimo!", 
                    receivedAt = DateTime.UtcNow 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false,
                    message = "Erro ao processar feedback", 
                    error = ex.Message 
                });
            }
        }
    }
}