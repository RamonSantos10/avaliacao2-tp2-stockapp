using System.Threading.Tasks;
using Moq;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Application.Services;
using Xunit;

public class ReviewServiceTests
{
    [Fact]
    public async Task AddReviewAsync_Should_Call_SentimentAnalysis_And_Save_Review()
    {
        // Arrange
        var mockReviewRepo = new Mock<IReviewRepository>();
        var mockSentimentService = new Mock<ISentimentAnalysisService>();

        var expectedSentiment = "Positive";
        mockSentimentService
            .Setup(s => s.AnalyzeSentiment(It.IsAny<string>()))
            .Returns(expectedSentiment);

        var reviewService = new ReviewService(mockReviewRepo.Object, mockSentimentService.Object);

        int productId = 1;
        string userId = "user123";
        int rating = 5;
        string comment = "Muito bom produto!";

        // Act
        await reviewService.AddReviewAsync(productId, userId, rating, comment);

        // Assert
        mockSentimentService.Verify(s => s.AnalyzeSentiment(comment), Times.Once);

        mockReviewRepo.Verify(r => r.AddAsync(It.Is<Review>(review =>
            review.ProductId == productId &&
            review.UserId == userId &&
            review.Rating == rating &&
            review.Comment == comment &&
            review.Sentiment == expectedSentiment
        )), Times.Once);
    }
}
