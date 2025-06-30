using Microsoft.AspNetCore.Mvc;
using StockApp.Application.Interfaces;

namespace StockApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountsController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet("apply")]
        public ActionResult<decimal> ApplyDiscount(decimal price, decimal discountPercentage)
        {
            var result = _discountService.ApplyDiscount(price, discountPercentage);
            return Ok(result);
        }
    }
}
