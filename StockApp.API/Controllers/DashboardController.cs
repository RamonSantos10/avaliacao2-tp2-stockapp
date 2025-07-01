using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using StockApp.Application.DTOs; 
using StockApp.Infra.Data.Context;

namespace StockApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context; 

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard-stock")]
        public async Task<IActionResult> GetDashboardstockData()
        {
            var dashboardData = new DashboardstockDTO
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalStockValue = await _context.Products.SumAsync(p => p.Stock * p.Price),
                LowStockProducts = await _context.Products
                    .Where(p => p.Stock < 10)
                    .Select(p => new ProductstockDTO
                    {
                        ProductName = p.Name,
                        Stock = p.Stock
                    })
                    .ToListAsync()
            };

            return Ok(dashboardData);
        }
    }
}