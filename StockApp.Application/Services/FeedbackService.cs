using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;

namespace StockApp.Application.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly ISentimentAnalysisService _sentimentAnalysisService;
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackService(ISentimentAnalysisService sentimentAnalysisService, IFeedbackRepository feedbackRepository)
        {
            _sentimentAnalysisService = sentimentAnalysisService;
            _feedbackRepository = feedbackRepository;
        }

        public async Task SubmitFeedbackAsync(string userId, string feedbackMessage)
        {
            var sentiment = _sentimentAnalysisService.AnalyzeSentiment(feedbackMessage);

            var feedback = new Feedback
            {
                UserId = userId,
                FeedbackText = feedbackMessage,
                Sentiment = sentiment
            };

            
            await _feedbackRepository.SaveAsync(feedback);
        }
    }
}
