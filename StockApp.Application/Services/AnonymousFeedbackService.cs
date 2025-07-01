using StockApp.Application.Interfaces;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;

namespace StockApp.Application.Services
{
    public class AnonymousFeedbackService : IAnonymousFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ISentimentAnalysisService _sentimentAnalysisService;

        public AnonymousFeedbackService(IFeedbackRepository feedbackRepository, ISentimentAnalysisService sentimentAnalysisService)
        {
            _feedbackRepository = feedbackRepository;
            _sentimentAnalysisService = sentimentAnalysisService;
        }

        public async Task CollectFeedbackAsync(string feedback)
        {
            // Análise de sentimento do feedback anônimo
            var sentiment = _sentimentAnalysisService.AnalyzeSentiment(feedback);

            // Criar feedback anônimo (sem UserId)
            var anonymousFeedback = new Feedback
            {
                UserId = "ANONYMOUS", // Identificador para feedback anônimo
                FeedbackText = feedback,
                Sentiment = sentiment,
                CreatedAt = DateTime.UtcNow
            };

            // Salvar no repositório
            await _feedbackRepository.SaveAsync(anonymousFeedback);
        }
    }
}