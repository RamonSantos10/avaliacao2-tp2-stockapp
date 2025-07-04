using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task UpdateAsync(Product product)
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

            if (product.Category != null)
            {
                _productContext.Entry(product.Category).State = EntityState.Unchanged;
            }

            await _productContext.SaveChangesAsync();
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

        public async Task<IEnumerable<Product>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _productContext.Products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByIdsAsync(List<int> productIds)
        {
            return await _productContext.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetLowStockAsync(int threshold)
        {
            return await _productContext.Products
                .Where(p => p.Stock <= threshold)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _productContext.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _productContext.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product product)
        {
            _productContext.Add(product);
            await _productContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productContext.Products.FindAsync(id);
            if (product != null)
            {
                _productContext.Products.Remove(product);
                await _productContext.SaveChangesAsync();
            }
        }

        public async Task BulkUpdateAsync(List<Product> products)
        {
            foreach (var product in products)
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

                if (product.Category != null)
                {
                    _productContext.Entry(product.Category).State = EntityState.Unchanged;
                }
            }

            await _productContext.SaveChangesAsync();
        }
    }
}
