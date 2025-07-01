using StockApp.Domain.Entities;

namespace StockApp.Domain.Interfaces
{
    public interface IAnonymousFeedbackRepository
    {
        Task<AnonymousFeedback> AddAsync(AnonymousFeedback anonymousFeedback);
        Task<IEnumerable<AnonymousFeedback>> GetAllAsync();
        Task<AnonymousFeedback?> GetByIdAsync(int id);
        Task<IEnumerable<AnonymousFeedback>> GetBySentimentAsync(string sentiment);
        Task<IEnumerable<AnonymousFeedback>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}