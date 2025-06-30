using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

public class InMemoryFeedbackRepository : IFeedbackRepository
{
    private readonly List<Feedback> _feedbacks = new();

    public Task SaveAsync(Feedback feedback)
    {
        _feedbacks.Add(feedback);
        return Task.CompletedTask;
    }

    public IEnumerable<Feedback> GetAll() => _feedbacks;
}
