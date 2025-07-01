namespace StockApp.Application.Interfaces
{
    public interface IAnonymousFeedbackService
    {
        Task CollectFeedbackAsync(string feedback);
    }
}