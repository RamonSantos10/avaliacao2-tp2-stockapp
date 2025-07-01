using StockApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Domain.Interfaces
{
    public interface IEmployeeEvaluationRepository
    {
        Task<IEnumerable<EmployeeEvaluation>> GetAllAsync();
        Task<EmployeeEvaluation> GetByIdAsync(int id);
        Task<EmployeeEvaluation> CreateAsync(EmployeeEvaluation evaluation);
        Task<EmployeeEvaluation> UpdateAsync(EmployeeEvaluation evaluation);
        Task DeleteAsync(int id);
        Task<IEnumerable<EmployeeEvaluation>> GetByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<EmployeeEvaluation>> GetByPeriodAsync(string period);
        Task<EmployeeEvaluation> GetLatestByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<EmployeeEvaluation>> GetByScoreRangeAsync(int minScore, int maxScore);
        Task<bool> ExistsAsync(int id);
    }
}