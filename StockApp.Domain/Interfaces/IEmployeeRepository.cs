using StockApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockApp.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee> GetByIdAsync(int id);
        Task<Employee> CreateAsync(Employee employee);
        Task<Employee> UpdateAsync(Employee employee);
        Task DeleteAsync(int id);
        Task<IEnumerable<Employee>> GetByDepartmentAsync(string department);
        Task<IEnumerable<Employee>> GetActiveEmployeesAsync();
        Task<bool> ExistsAsync(int id);
    }
}