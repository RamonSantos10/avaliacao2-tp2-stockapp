using Microsoft.AspNetCore.Mvc;
using StockApp.Application.Services;
using StockApp.Application.DTOs;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController: ControllerBase
    {
        private readonly ReviewService _reviewService;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductsController(ReviewService reviewService, IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _reviewService = reviewService;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create(Product product)
        {
            // Validar se o CategoryId existe
            if (product.CategoryId > 0)
            {
                var categoryExists = await _categoryRepository.GetById(product.CategoryId);
                if (categoryExists == null)
                {
                    return BadRequest($"Category with ID {product.CategoryId} does not exist. Please provide a valid CategoryId.");
                }
            }
            else
            {
                return BadRequest("CategoryId is required and must be greater than 0.");
            }

            await _productRepository.AddAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
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
