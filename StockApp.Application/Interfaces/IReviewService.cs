using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    public interface IReviewService
    {
        Task AddReviewAsync(int productId, string userId, int rating, string comment);
    }
}