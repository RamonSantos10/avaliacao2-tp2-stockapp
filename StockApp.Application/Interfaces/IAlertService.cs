using System.Threading.Tasks;

namespace StockApp.Application.Interfaces.Services
{
    public interface IAlertService
    {
        Task SendAlertAsync(string userId, string message);
    }
}
