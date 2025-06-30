using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockApp.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace StockApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IJustInTimeInventoryService _justInTimeInventoryService;
        private readonly IInventoryService _inventoryService;

        public InventoryController(IJustInTimeInventoryService justInTimeInventoryService, IInventoryService inventoryService)
        {
            _justInTimeInventoryService = justInTimeInventoryService;
            _inventoryService = inventoryService;
        }
        [HttpPost("optimize-jit")]
        public async Task<IActionResult> OptimizeJustInTimeInventory()
        {
            try
            {
                await _justInTimeInventoryService.OptimizeInventoryAsync();
                return Ok("Otimização de inventário Just-in-Time concluída com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao otimizar inventário JIT: {ex.Message}");
                return StatusCode(500, $"Ocorreu um erro interno ao otimizar o inventário: {ex.Message}");
            }
        }

        /// <summary>
        /// Test endpoint to verify API is working
        /// </summary>
        /// <returns>Success message</returns>
        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult TestEndpoint()
        {
            return Ok(new { message = "Inventory API is working!", timestamp = DateTime.Now });
        }

        /// <summary>
        /// Replenish stock for products with low inventory using default settings
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("replenish")]
        public async Task<IActionResult> ReplenishStock()
        {
            try
            {
                await _inventoryService.ReplenishStockAsync();
                return Ok(new { message = "Stock replenishment completed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error during stock replenishment", error = ex.Message });
            }
        }

        /// <summary>
        /// Replenish stock for products with low inventory using custom threshold and quantity
        /// </summary>
        /// <param name="threshold">Stock threshold to consider as low stock</param>
        /// <param name="replenishQuantity">Quantity to add to each low stock product</param>
        /// <returns>Success message</returns>
        [HttpPost("replenish/{threshold}/{replenishQuantity}")]
        public async Task<IActionResult> ReplenishStock(int threshold, int replenishQuantity)
        {
            try
            {
                if (threshold < 0 || replenishQuantity <= 0)
                {
                    return BadRequest(new { message = "Invalid parameters. Threshold must be >= 0 and replenish quantity must be > 0" });
                }

                await _inventoryService.ReplenishStockAsync(threshold, replenishQuantity);
                return Ok(new { message = $"Stock replenishment completed successfully. Threshold: {threshold}, Quantity added: {replenishQuantity}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error during stock replenishment", error = ex.Message });
            }
        }
    }
}