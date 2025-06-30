using Microsoft.EntityFrameworkCore;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Context;
using System;

namespace StockApp.Infra.Data.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Review>> GetByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();
        }
    }
}
