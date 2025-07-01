using System.Collections.Generic;

namespace StockApp.Application.DTOs
{
    public class DashboardstockDTO
    {
        public int TotalProducts { get; set; }
        public decimal TotalStockValue { get; set; }
        public List<ProductstockDTO> LowStockProducts { get; set; }
    }
}