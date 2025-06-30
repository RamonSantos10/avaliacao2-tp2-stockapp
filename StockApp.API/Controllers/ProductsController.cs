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
        private readonly IMapper _mapper;

        public ProductsController(ReviewService reviewService, IProductRepository productRepository, IMapper mapper)
        {
            _reviewService = reviewService;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> Create(ProductCreateDTO productCreateDto)
        {
            var product = _mapper.Map<Product>(productCreateDto);
            await _productRepository.AddAsync(product);
            var createdProductDto = _mapper.Map<ProductDTO>(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, createdProductDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var productDto = _mapper.Map<ProductDTO>(product);
            return Ok(productDto);
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
