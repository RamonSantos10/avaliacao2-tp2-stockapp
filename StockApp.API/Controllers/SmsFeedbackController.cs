using Microsoft.AspNetCore.Mvc;
using StockApp.Application.Interfaces;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmsFeedbackController : ControllerBase
    {
        private readonly ISmsFeedbackService _smsFeedbackService;

        public SmsFeedbackController(ISmsFeedbackService smsFeedbackService)
        {
            _smsFeedbackService = smsFeedbackService;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitFeedback([FromBody] SmsFeedbackInputModel model)
        {
            await _smsFeedbackService.SubmitFeedbackAsync(model.PhoneNumber, model.Feedback);
            return Ok(new { message = "Feedback processado com sucesso!" });
        }
    }

    public class SmsFeedbackInputModel
    {
        public string PhoneNumber { get; set; }
        public string Feedback { get; set; }
    }

}
