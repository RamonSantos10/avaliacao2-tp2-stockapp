using StockApp.Domain.Entities;

namespace StockApp.Application.Interfaces
{
    public interface IAnonymousFeedbackService
    {
        Task CollectFeedbackAsync(string feedback);
        Task CollectFeedbackAsync(string feedback, string? ipAddress, string? userAgent);
        Task<IEnumerable<AnonymousFeedback>> GetAllFeedbacksAsync();
        Task<IEnumerable<AnonymousFeedback>> GetFeedbacksBySentimentAsync(string sentiment);
    }
}