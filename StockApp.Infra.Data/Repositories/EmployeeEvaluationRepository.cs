using Microsoft.EntityFrameworkCore;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockApp.Infra.Data.Repositories
{
    public class EmployeeEvaluationRepository : IEmployeeEvaluationRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeEvaluationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeEvaluation>> GetAllAsync()
        {
            return await _context.EmployeeEvaluations
                .Include(e => e.Employee)
                .ToListAsync();
        }

        public async Task<EmployeeEvaluation> GetByIdAsync(int id)
        {
            return await _context.EmployeeEvaluations
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<EmployeeEvaluation> CreateAsync(EmployeeEvaluation evaluation)
        {
            _context.EmployeeEvaluations.Add(evaluation);
            await _context.SaveChangesAsync();
            return evaluation;
        }

        public async Task<EmployeeEvaluation> UpdateAsync(EmployeeEvaluation evaluation)
        {
            evaluation.UpdatedAt = System.DateTime.UtcNow;
            _context.Entry(evaluation).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return evaluation;
        }

        public async Task DeleteAsync(int id)
        {
            var evaluation = await _context.EmployeeEvaluations.FindAsync(id);
            if (evaluation != null)
            {
                _context.EmployeeEvaluations.Remove(evaluation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<EmployeeEvaluation>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.EmployeeEvaluations
                .Include(e => e.Employee)
                .Where(e => e.EmployeeId == employeeId)
                .OrderByDescending(e => e.EvaluationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeEvaluation>> GetByPeriodAsync(string period)
        {
            return await _context.EmployeeEvaluations
                .Include(e => e.Employee)
                .Where(e => e.EvaluationPeriod == period)
                .ToListAsync();
        }

        public async Task<EmployeeEvaluation> GetLatestByEmployeeIdAsync(int employeeId)
        {
            return await _context.EmployeeEvaluations
                .Include(e => e.Employee)
                .Where(e => e.EmployeeId == employeeId)
                .OrderByDescending(e => e.EvaluationDate)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<EmployeeEvaluation>> GetByScoreRangeAsync(int minScore, int maxScore)
        {
            return await _context.EmployeeEvaluations
                .Include(e => e.Employee)
                .Where(e => e.EvaluationScore >= minScore && e.EvaluationScore <= maxScore)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.EmployeeEvaluations.AnyAsync(e => e.Id == id);
        }
    }
}