using Xunit;
using Moq;
using StockApp.Application.Services;
using StockApp.Domain.Interfaces;
using StockApp.Domain.Entities;
using System.Threading.Tasks;

namespace StockApp.Domain.Test.Services
{
    public class AnonymousFeedbackServiceTests
    {
        private readonly Mock<IFeedbackRepository> _mockFeedbackRepository;
        private readonly Mock<ISentimentAnalysisService> _mockSentimentAnalysisService;
        private readonly AnonymousFeedbackService _anonymousFeedbackService;

        public AnonymousFeedbackServiceTests()
        {
            _mockFeedbackRepository = new Mock<IFeedbackRepository>();
            _mockSentimentAnalysisService = new Mock<ISentimentAnalysisService>();
            _anonymousFeedbackService = new AnonymousFeedbackService(
                _mockFeedbackRepository.Object,
                _mockSentimentAnalysisService.Object
            );
        }

        [Fact]
        public async Task CollectFeedbackAsync_ShouldSaveFeedbackWithAnonymousUserId()
        {
            // Arrange
            var feedbackText = "Produto excelente, muito satisfeito!";
            var expectedSentiment = "Positive";
            
            _mockSentimentAnalysisService
                .Setup(x => x.AnalyzeSentiment(feedbackText))
                .Returns(expectedSentiment);

            _mockFeedbackRepository
                .Setup(x => x.SaveAsync(It.IsAny<Feedback>()))
                .Returns(Task.CompletedTask);

            // Act
            await _anonymousFeedbackService.CollectFeedbackAsync(feedbackText);

            // Assert
            _mockSentimentAnalysisService.Verify(x => x.AnalyzeSentiment(feedbackText), Times.Once);
            _mockFeedbackRepository.Verify(x => x.SaveAsync(It.Is<Feedback>(f => 
                f.UserId == "ANONYMOUS" &&
                f.FeedbackText == feedbackText &&
                f.Sentiment == expectedSentiment
            )), Times.Once);
        }

        [Fact]
        public async Task CollectFeedbackAsync_ShouldAnalyzeSentimentBeforeSaving()
        {
            // Arrange
            var feedbackText = "Serviço ruim, não recomendo";
            var expectedSentiment = "Negative";
            
            _mockSentimentAnalysisService
                .Setup(x => x.AnalyzeSentiment(feedbackText))
                .Returns(expectedSentiment);

            // Act
            await _anonymousFeedbackService.CollectFeedbackAsync(feedbackText);

            // Assert
            _mockSentimentAnalysisService.Verify(x => x.AnalyzeSentiment(feedbackText), Times.Once);
            _mockFeedbackRepository.Verify(x => x.SaveAsync(It.Is<Feedback>(f => 
                f.Sentiment == expectedSentiment
            )), Times.Once);
        }

        [Theory]
        [InlineData("Produto muito bom!")]
        [InlineData("Atendimento excelente, parabéns!")]
        [InlineData("Entrega rápida e produto de qualidade")]
        public async Task CollectFeedbackAsync_ShouldHandleDifferentFeedbackTexts(string feedbackText)
        {
            // Arrange
            _mockSentimentAnalysisService
                .Setup(x => x.AnalyzeSentiment(It.IsAny<string>()))
                .Returns("Positive");

            // Act
            await _anonymousFeedbackService.CollectFeedbackAsync(feedbackText);

            // Assert
            _mockFeedbackRepository.Verify(x => x.SaveAsync(It.Is<Feedback>(f => 
                f.FeedbackText == feedbackText &&
                f.UserId == "ANONYMOUS"
            )), Times.Once);
        }
    }
}