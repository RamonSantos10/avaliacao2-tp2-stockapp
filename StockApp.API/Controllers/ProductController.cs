using Microsoft.AspNetCore.Mvc;
using StockApp.Application.DTOs;
using StockApp.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    [HttpPost("compare")]
    [ProducesResponseType(typeof(IEnumerable<ProductDTO>), 200)] 
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> CompareProducts([FromBody] List<int> productIds) // Alterado para ProductDTO
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
}
