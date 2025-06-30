namespace StockApp.Application.Interfaces
{
    public interface ISmsFeedbackService
    {
        Task SubmitFeedbackAsync(string userId, string message);
        Task CollectFeedbackAsync(string phoneNumber, string feedback);
    }
}
