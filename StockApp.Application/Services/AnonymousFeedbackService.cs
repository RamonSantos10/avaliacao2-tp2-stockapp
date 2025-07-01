using StockApp.Application.Interfaces;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;

namespace StockApp.Application.Services
{
    public class AnonymousFeedbackService : IAnonymousFeedbackService
    {
        private readonly IAnonymousFeedbackRepository _anonymousFeedbackRepository;
        private readonly ISentimentAnalysisService _sentimentAnalysisService;

        public AnonymousFeedbackService(IAnonymousFeedbackRepository anonymousFeedbackRepository, ISentimentAnalysisService sentimentAnalysisService)
        {
            _anonymousFeedbackRepository = anonymousFeedbackRepository;
            _sentimentAnalysisService = sentimentAnalysisService;
        }

        public async Task CollectFeedbackAsync(string feedback)
        {
            // Análise de sentimento do feedback anônimo
            var sentiment = _sentimentAnalysisService.AnalyzeSentiment(feedback);

            // Criar feedback anônimo usando a entidade específica
            var anonymousFeedback = new AnonymousFeedback
            {
                FeedbackText = feedback,
                Sentiment = sentiment,
                CreatedAt = DateTime.UtcNow
            };

            await _anonymousFeedbackRepository.AddAsync(anonymousFeedback);
        }

        public async Task CollectFeedbackAsync(string feedback, string? ipAddress, string? userAgent)
        {
            // Análise de sentimento do feedback anônimo
            var sentiment = _sentimentAnalysisService.AnalyzeSentiment(feedback);

            // Criar feedback anônimo completo
            var anonymousFeedback = new AnonymousFeedback
            {
                FeedbackText = feedback,
                Sentiment = sentiment,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                CreatedAt = DateTime.UtcNow
            };

            await _anonymousFeedbackRepository.AddAsync(anonymousFeedback);
        }

        public async Task<IEnumerable<AnonymousFeedback>> GetAllFeedbacksAsync()
        {
            return await _anonymousFeedbackRepository.GetAllAsync();
        }

        public async Task<IEnumerable<AnonymousFeedback>> GetFeedbacksBySentimentAsync(string sentiment)
        {
            return await _anonymousFeedbackRepository.GetBySentimentAsync(sentiment);
        }
    }
}