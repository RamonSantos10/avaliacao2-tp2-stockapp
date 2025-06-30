namespace StockApp.Application.Interfaces
{
    public interface ISmsFeedbackService
    {
        Task CollectFeedbackAsync(string phoneNumber, string feedback);
    }
}
