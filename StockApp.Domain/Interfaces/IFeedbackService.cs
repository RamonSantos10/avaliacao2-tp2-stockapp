namespace StockApp.Domain.Interfaces
{
    public interface IFeedbackService
    {
        Task SubmitFeedbackAsync(string userId, string feedbackMessage);
    }
}
