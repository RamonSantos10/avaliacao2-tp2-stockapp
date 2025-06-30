using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
namespace StockApp.Infra.Data.Repositories
{
    public class InMemoryFeedbackRepository : IFeedbackRepository
    {
        private readonly List<Feedback> _storage = new();
        public InMemoryFeedbackRepository()
        {
        }

        public Task AddAsync(Feedback feedback)
        {
            _storage.Add(feedback);
            return Task.CompletedTask;
        }
    }
}
