using Microsoft.AspNetCore.Mvc;
using StockApp.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace StockApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IJustInTimeInventoryService _justInTimeInventoryService;

        public InventoryController(IJustInTimeInventoryService justInTimeInventoryService)
        {
            _justInTimeInventoryService = justInTimeInventoryService;
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
    }
}