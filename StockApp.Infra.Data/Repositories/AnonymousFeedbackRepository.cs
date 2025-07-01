using Microsoft.EntityFrameworkCore;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Context;

namespace StockApp.Infra.Data.Repositories
{
    public class AnonymousFeedbackRepository : IAnonymousFeedbackRepository
    {
        private readonly ApplicationDbContext _context;

        public AnonymousFeedbackRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AnonymousFeedback> AddAsync(AnonymousFeedback anonymousFeedback)
        {
            _context.AnonymousFeedbacks.Add(anonymousFeedback);
            await _context.SaveChangesAsync();
            return anonymousFeedback;
        }

        public async Task<IEnumerable<AnonymousFeedback>> GetAllAsync()
        {
            return await _context.AnonymousFeedbacks
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<AnonymousFeedback?> GetByIdAsync(int id)
        {
            return await _context.AnonymousFeedbacks
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IEnumerable<AnonymousFeedback>> GetBySentimentAsync(string sentiment)
        {
            return await _context.AnonymousFeedbacks
                .Where(f => f.Sentiment == sentiment)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<AnonymousFeedback>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.AnonymousFeedbacks
                .Where(f => f.CreatedAt >= startDate && f.CreatedAt <= endDate)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }
    }
}