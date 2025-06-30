using Microsoft.AspNetCore.Mvc;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] string? name,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            var products = await _service.SearchAsync(name, minPrice, maxPrice);
            return Ok(products);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var products = await _service.GetAllAsync(pageNumber, pageSize);
            return Ok(products);
        }

        [HttpPost("compare")]
        [ProducesResponseType(typeof(IEnumerable<ProductDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> CompareProducts([FromBody] List<int> productIds)
        {
            if (productIds == null || !productIds.Any())
            {
                return BadRequest("A lista de IDs de produtos não pode estar vazia.");
            }

            var products = await _service.GetProductsByIdsAsync(productIds);

            if (products == null || !products.Any())
            {
                return NotFound("Nenhum produto encontrado para os IDs fornecidos.");
            }

            return Ok(products);
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetLowStock([FromQuery] int threshold)
        {
            var products = await _service.GetLowStockAsync(threshold);
            return Ok(products);
        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportToCsv()
        {
            var products = await _service.GetProducts();
            var csv = new StringBuilder();

            csv.AppendLine("Id,Name,Description,Price,Stock");

            foreach (var product in products)
            {
                var line = string.Format("{0},\"{1}\",\"{2}\",{3},{4}",
                     product.Id,
                     product.Name,
                     product.Description,
                     product.Price.ToString(System.Globalization.CultureInfo.InvariantCulture),
                     product.Stock);
                csv.AppendLine(line);
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());

            return File(bytes, "text/csv", "products.csv");
        }

    }
}
