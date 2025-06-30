using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;

namespace StockApp.Application.Services
{
    public class ReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task AddReviewAsync(int productId, string userId, int rating, string comment)
        {
            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Comment = comment,
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
