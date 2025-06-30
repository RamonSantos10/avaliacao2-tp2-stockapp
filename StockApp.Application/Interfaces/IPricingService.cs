using System.Threading.Tasks;
using StockApp.Application.DTOs;

namespace StockApp.Application.Interfaces
{
    public interface IPricingService
    {
        Task<ProductPricingDTO> GetProductDetailsAsync(string productId);
    }
}