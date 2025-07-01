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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Create(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                return BadRequest("Product name is required and cannot be empty.");
            }

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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest("The ID in the URL does not match the product ID.");
            }

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                return BadRequest("Product name is required and cannot be empty.");
            }

            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

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

            await _productRepository.UpdateAsync(product);
            return NoContent();
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            await _productRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpPut("bulk-update")]
        public async Task<IActionResult> BulkUpdate([FromBody] List<Product> products)
        {
            if (products == null || products.Count == 0)
            {
                return BadRequest("A lista de produtos não pode estar vazia.");
            }

            // Validar se todos os produtos existem
            foreach (var product in products)
            {
                if (string.IsNullOrWhiteSpace(product.Name))
                {
                    return BadRequest($"O nome do produto com ID {product.Id} é obrigatório.");
                }

                var existingProduct = await _productRepository.GetByIdAsync(product.Id);
                if (existingProduct == null)
                {
                    return NotFound($"Produto com ID {product.Id} não encontrado.");
                }

                if (product.CategoryId > 0)
                {
                    var categoryExists = await _categoryRepository.GetById(product.CategoryId);
                    if (categoryExists == null)
                    {
                        return BadRequest($"Categoria com ID {product.CategoryId} não existe para o produto {product.Id}.");
                    }
                }
                else
                {
                    return BadRequest($"CategoryId é obrigatório para o produto {product.Id}.");
                }
            }

            await _productRepository.BulkUpdateAsync(products);
            return Ok(new { message = $"{products.Count} produtos atualizados com sucesso!" });
        }

    }
}
