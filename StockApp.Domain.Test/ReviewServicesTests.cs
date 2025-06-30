using Xunit;
using Moq;
using System.Threading.Tasks;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Application.Services;

namespace StockApp.Domain.Test
{
    public class ReviewServicesTests
    {
        [Fact]
        public async Task SubmitReviewAsync_ShouldAddReviewCorrectly()
        {
            // Arrange
            var mockRepo = new Mock<IReviewRepository>();
            var service = new ReviewService(mockRepo.Object);

            int productId = 1;
            string userId = "user123";
            int rating = 5;
            string comment = "Excelente!";

            // Act
            await service.AddReviewAsync(productId, userId, rating, comment);

            // Assert
            mockRepo.Verify(repo => repo.AddAsync(It.Is<Review>(r =>
                r.ProductId == productId &&
                r.UserId == userId &&
                r.Rating == rating &&
                r.Comment == comment &&
                r.Date != default
            )), Times.Once);
        }
        }
}
