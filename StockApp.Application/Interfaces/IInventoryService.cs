using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    public interface IInventoryService
    {
        Task ReplenishStockAsync();
        Task ReplenishStockAsync(int threshold, int replenishQuantity);
    }
}