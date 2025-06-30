using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace StockApp.Infra.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        ApplicationDbContext _productContext;
        public ProductRepository(ApplicationDbContext context)
        {
            _productContext = context;
        }

        public async Task<Product> Create(Product product)
        {
            _productContext.Add(product);
            await _productContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product> GetById(int? id)
        {
            return await _productContext.Products.Include(p => p.Category).SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _productContext.Products
                .Include(p => p.Category) 
                .ToListAsync();
        }

        public async Task<Product> Remove(Product product)
        {
            _productContext.Remove(product);
            await _productContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product> Update(Product product)
        {
            var existingEntity = _productContext.Products.Local.FirstOrDefault(p => p.Id == product.Id);

            if (existingEntity != null)
            {
                _productContext.Entry(existingEntity).CurrentValues.SetValues(product);
            }
            else
            {
                _productContext.Attach(product);
                _productContext.Entry(product).State = EntityState.Modified;
            }

            _productContext.Entry(product.Category).State = EntityState.Unchanged;


            await _productContext.SaveChangesAsync();
            return product;
        }

        public async Task<IEnumerable<Product>> SearchAsync(string name, decimal? minPrice, decimal? maxPrice)
        {
            var query = _productContext.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(p => p.Name.Contains(name));

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            return await query.ToListAsync();
        }
    }
}