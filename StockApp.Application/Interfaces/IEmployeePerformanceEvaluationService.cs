using StockApp.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    public interface IEmployeePerformanceEvaluationService
    {
        Task<EmployeeEvaluationDTO> EvaluatePerformanceAsync(int employeeId);
        Task<EmployeeEvaluationDTO> CreateEvaluationAsync(CreateEmployeeEvaluationDTO createEvaluationDto);
        Task<EmployeeEvaluationDTO> UpdateEvaluationAsync(int id, CreateEmployeeEvaluationDTO updateEvaluationDto);
        Task<EmployeeEvaluationDTO> GetEvaluationByIdAsync(int id);
        Task<IEnumerable<EmployeeEvaluationDTO>> GetAllEvaluationsAsync();
        Task<IEnumerable<EmployeeEvaluationDTO>> GetEvaluationsByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<EmployeeEvaluationDTO>> GetEvaluationsByPeriodAsync(string period);
        Task<EmployeeEvaluationDTO> GetLatestEvaluationByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<EmployeeEvaluationDTO>> GetEvaluationsByScoreRangeAsync(int minScore, int maxScore);
        Task DeleteEvaluationAsync(int id);
        Task<EmployeeDTO> GetEmployeeByIdAsync(int employeeId);
        Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync();
    }
}