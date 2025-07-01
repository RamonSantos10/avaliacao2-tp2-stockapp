using System.Threading.Tasks;
using Moq;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;
using StockApp.Application.Services;
using Xunit;

public class ReviewServiceTests
{
    private readonly Mock<IReviewRepository> _mockReviewRepo;
    private readonly Mock<ISentimentAnalysisService> _mockSentimentService;
    private readonly ReviewService _reviewService;

    public ReviewServiceTests()
    {
        _mockReviewRepo = new Mock<IReviewRepository>();
        _mockSentimentService = new Mock<ISentimentAnalysisService>();
        _reviewService = new ReviewService(_mockReviewRepo.Object, _mockSentimentService.Object);
    }

    [Fact]
    public async Task AddReviewAsync_Should_Call_SentimentAnalysis_And_Save_Review()
    {
        // Arrange
        var expectedSentiment = "Positive";
        _mockSentimentService
            .Setup(s => s.AnalyzeSentiment(It.IsAny<string>()))
            .Returns(expectedSentiment);

        int productId = 1;
        string userId = "user123";
        int rating = 5;
        string comment = "Muito bom produto!";

        // Act
        await _reviewService.AddReviewAsync(productId, userId, rating, comment);

        // Assert
        _mockSentimentService.Verify(s => s.AnalyzeSentiment(comment), Times.Once);

        _mockReviewRepo.Verify(r => r.AddAsync(It.Is<Review>(review =>
            review.ProductId == productId &&
            review.UserId == userId &&
            review.Rating == rating &&
            review.Comment == comment &&
            review.Sentiment == expectedSentiment
        )), Times.Once);
    }

    [Theory]
    [InlineData(0)] // Rating muito baixo
    [InlineData(6)] // Rating muito alto
    public async Task AddReviewAsync_Should_Throw_Exception_For_Invalid_Rating(int invalidRating)
    {
        // Arrange
        int productId = 1;
        string userId = "user123";
        string comment = "Teste de review";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _reviewService.AddReviewAsync(productId, userId, invalidRating, comment)
        );
    }

    [Theory]
    [InlineData("")]     // Comentário vazio
    [InlineData(null)]   // Comentário nulo
    [InlineData("   ")] // Apenas espaços em branco
    public async Task AddReviewAsync_Should_Throw_Exception_For_Invalid_Comment(string invalidComment)
    {
        // Arrange
        int productId = 1;
        string userId = "user123";
        int rating = 4;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _reviewService.AddReviewAsync(productId, userId, rating, invalidComment)
        );
    }
}