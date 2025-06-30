using StockApp.Application.Interfaces;
using StockApp.Domain.Interfaces;
using System.Threading.Tasks;

namespace StockApp.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IProductRepository _productRepository;

        public InventoryService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task ReplenishStockAsync()
        {
            var lowStockProducts = await _productRepository.GetLowStockAsync(10); // threshold de exemplo
            foreach (var product in lowStockProducts)
            {
                product.Stock += 50; // quantidade de reposição de exemplo
                await _productRepository.Update(product);
            }
        }

        public async Task ReplenishStockAsync(int threshold, int replenishQuantity)
        {
            var lowStockProducts = await _productRepository.GetLowStockAsync(threshold);
            foreach (var product in lowStockProducts)
            {
                product.Stock += replenishQuantity;
                await _productRepository.Update(product);
            }
        }
    }
}