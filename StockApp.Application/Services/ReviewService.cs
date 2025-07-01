using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Application.Interfaces;

namespace StockApp.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ISentimentAnalysisService _sentimentService;

        public ReviewService(IReviewRepository reviewRepository, ISentimentAnalysisService sentimentService)
        {
            _reviewRepository = reviewRepository;
            _sentimentService = sentimentService;
        }

        public async Task AddReviewAsync(int productId, string userId, int rating, string comment)
        {
            var sentiment = _sentimentService.AnalyzeSentiment(comment);
            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Comment = comment,
                Sentiment = sentiment,
                Date = DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(review);
        }

        public async Task<IEnumerable<Review>> GetReviewsForProductAsync(int productId)
        {
            return await _reviewRepository.GetByProductIdAsync(productId);
        }
    }
}
