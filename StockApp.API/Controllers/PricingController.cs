using Microsoft.AspNetCore.Mvc;
using StockApp.Application.Interfaces;
using StockApp.Application.DTOs;
using System.Threading.Tasks;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PricingController : ControllerBase
    {
        private readonly IPricingService _pricingService;

        public PricingController(IPricingService pricingService)
        {
            _pricingService = pricingService;
        }

        /// <summary>
        /// Obtém informações completas de um produto através de API externa
        /// </summary>
        /// <param name="productId">ID do produto</param>
        /// <returns>Informações completas do produto (nome, descrição, preço, stock, categoria, marca)</returns>
        [HttpGet("{productId}")]
        public async Task<ActionResult<ProductPricingDTO>> GetProductDetails(string productId)
        {
            try
            {
                var productDetails = await _pricingService.GetProductDetailsAsync(productId);
                return Ok(productDetails);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest($"Erro ao obter informações do produto: {ex.Message}");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
    }
}