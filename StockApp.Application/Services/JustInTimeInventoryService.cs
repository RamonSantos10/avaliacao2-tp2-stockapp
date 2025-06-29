using StockApp.Application.Interfaces;
using StockApp.Domain.Interfaces;
using StockApp.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApp.Application.Services
{
    public class JustInTimeInventoryService : IJustInTimeInventoryService
    {
        private readonly IProductRepository _productRepository;

        public JustInTimeInventoryService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task OptimizeInventoryAsync()
        { 

            IEnumerable<Product> products = await _productRepository.GetProducts();

            foreach (var product in products)
            {

                // Lógica de exemplo: Se o estoque estiver acima de 10 unidades, reduza para 10.
                // Se estiver abaixo de 5, aumente para 10.
                int currentStock = product.Stock;
                int targetStock = 10; 

                if (currentStock > targetStock)
                {
                    // Reduzir estoque (simulando venda, devolução, etc)
                    product.Stock = targetStock;
                    await _productRepository.Update(product);
                    Console.WriteLine($"Produto '{product.Name}': Estoque ajustado de {currentStock} para {product.Stock} (redução JIT).");
                }
                else if (currentStock < targetStock / 2) 
                {
                    // Aumentar estoque (simulando nova compra, produção)
                    product.Stock = targetStock;
                    await _productRepository.Update(product);
                    Console.WriteLine($"Produto '{product.Name}': Estoque ajustado de {currentStock} para {product.Stock} (reposição JIT).");
                }
                else
                {
                    Console.WriteLine($"Produto '{product.Name}': Estoque atual ({currentStock}) dentro da faixa JIT. Nenhuma ação necessária.");
                }
            }
        }
    }
}