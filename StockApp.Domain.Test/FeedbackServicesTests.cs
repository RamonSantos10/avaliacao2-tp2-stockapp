using Xunit;
using Moq;
using StockApp.Domain.Interfaces;
using StockApp.Domain.Entities;
using StockApp.Application.Services;

namespace StockApp.Domain.Test
{
    public class FeedbackServicesTests
    {
        [Fact]
        public async Task SubmitFeedbackAsync_ShouldAnalyzeAndStoreFeedback()
        {
            // Arrange
            var sentimentService = new Mock<ISentimentAnalysisService>();
            var repo = new Mock<IFeedbackRepository>();
            sentimentService.Setup(s => s.AnalyzeSentiment(It.IsAny<string>())).Returns("Positivo");

            var service = new FeedbackService(sentimentService.Object, repo.Object);

            // Act
            await service.SubmitFeedbackAsync("user123", "Muito bom");

            // Assert
            repo.Verify(r => r.SaveAsync(It.Is<Feedback>(f =>
                f.UserId == "user123" &&
                f.FeedbackText == "Muito bom" &&
                f.Sentiment == "Positivo"
            )), Times.Once);
        }
    }
}
