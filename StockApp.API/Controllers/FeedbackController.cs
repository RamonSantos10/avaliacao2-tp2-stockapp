using Microsoft.AspNetCore.Mvc;
using StockApp.Domain.Interfaces;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] FeedbackInputModel model)
        {
            await _feedbackService.SubmitFeedbackAsync(model.UserId, model.Message);
            return Ok(new { message = "Feedback enviado com sucesso!" });
        }
    }
    public class FeedbackInputModel
    {
        public string UserId { get; set; }
        public string Message { get; set; }
    }

}
