
using Microsoft.AspNetCore.Mvc;
using StockApp.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class AlertsController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public AlertsController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        [HttpPost] 
        [Route("send")] 
        public async Task<IActionResult> SendAlert([FromQuery] string userId, [FromQuery] string message)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(message))
            {
                return BadRequest("UserId e Message são obrigatórios para enviar um alerta.");
            }

            await _alertService.SendAlertAsync(userId, message);

            return Ok($"Alerta enviado com sucesso para o usuário {userId}.");
        }
    }
}