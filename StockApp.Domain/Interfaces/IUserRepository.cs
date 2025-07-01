using System.Threading.Tasks;
using StockApp.Domain.Entities;

namespace StockApp.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByUsernameAsync(string username);
        Task CreateAsync(User user);
    }
}