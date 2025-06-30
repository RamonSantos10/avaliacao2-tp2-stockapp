using StockApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> GetById(int? id);
        Task<Product> Create(Product product);
        Task<Product> Update(Product product);
        Task<Product> Remove(Product product);
        Task<IEnumerable<Product>> GetAllAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Product>> SearchAsync(string name, decimal? minPrice, decimal? maxPrice);
        Task<IEnumerable<Product>> GetByIdsAsync(List<int> productIds);
    }
}
