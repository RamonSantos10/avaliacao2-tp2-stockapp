using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(string username, string password);
    }
}