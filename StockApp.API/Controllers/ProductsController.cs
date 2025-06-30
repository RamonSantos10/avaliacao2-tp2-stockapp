using Microsoft.AspNetCore.Mvc;
using StockApp.Application.Services;
using StockApp.Application.DTOs;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController: ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ProductsController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost("{productId}/review")]
        public async Task<IActionResult> AddReview(int productId, [FromBody] ReviewInputModel model)
        {
            await _reviewService.AddReviewAsync(productId, model.UserId, model.Rating, model.Comment);
            return Ok(new { message = "Avaliação enviada com sucesso!" });
        }

        [HttpGet("{productId}/reviews")]
        public async Task<IActionResult> GetReviews(int productId)
        {
            var reviews = await _reviewService.GetReviewsForProductAsync(productId);
            return Ok(reviews);
        }
    }
}
