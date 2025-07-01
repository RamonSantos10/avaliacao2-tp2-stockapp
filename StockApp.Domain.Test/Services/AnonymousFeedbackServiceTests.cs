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
        private readonly Mock<IAnonymousFeedbackRepository> _mockAnonymousFeedbackRepository;
        private readonly Mock<ISentimentAnalysisService> _mockSentimentAnalysisService;
        private readonly AnonymousFeedbackService _anonymousFeedbackService;

        public AnonymousFeedbackServiceTests()
        {
            _mockAnonymousFeedbackRepository = new Mock<IAnonymousFeedbackRepository>();
            _mockSentimentAnalysisService = new Mock<ISentimentAnalysisService>();
            _anonymousFeedbackService = new AnonymousFeedbackService(
                _mockAnonymousFeedbackRepository.Object,
                _mockSentimentAnalysisService.Object
            );
        }

        [Fact]
        public async Task CollectFeedbackAsync_ShouldSaveAnonymousFeedback()
        {
            // Arrange
            var feedbackText = "Produto excelente, muito satisfeito!";
            var expectedSentiment = "Positive";
            
            _mockSentimentAnalysisService
                .Setup(x => x.AnalyzeSentiment(feedbackText))
                .Returns(expectedSentiment);

            _mockAnonymousFeedbackRepository
                .Setup(x => x.AddAsync(It.IsAny<AnonymousFeedback>()))
                .ReturnsAsync(new AnonymousFeedback());

            // Act
            await _anonymousFeedbackService.CollectFeedbackAsync(feedbackText);

            // Assert
            _mockSentimentAnalysisService.Verify(x => x.AnalyzeSentiment(feedbackText), Times.Once);
            _mockAnonymousFeedbackRepository.Verify(x => x.AddAsync(It.Is<AnonymousFeedback>(f => 
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
            _mockAnonymousFeedbackRepository.Verify(x => x.AddAsync(It.Is<AnonymousFeedback>(f => 
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
            _mockAnonymousFeedbackRepository.Verify(x => x.AddAsync(It.Is<AnonymousFeedback>(f => 
                f.FeedbackText == feedbackText
            )), Times.Once);
        }

        [Fact]
        public async Task CollectFeedbackAsync_WithIpAndUserAgent_ShouldSaveCompleteAnonymousFeedback()
        {
            // Arrange
            var feedbackText = "Produto excelente!";
            var ipAddress = "192.168.1.100";
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
            var expectedSentiment = "Positive";
            
            _mockSentimentAnalysisService
                .Setup(x => x.AnalyzeSentiment(feedbackText))
                .Returns(expectedSentiment);

            _mockAnonymousFeedbackRepository
                .Setup(x => x.AddAsync(It.IsAny<AnonymousFeedback>()))
                .ReturnsAsync(new AnonymousFeedback());

            // Act
            await _anonymousFeedbackService.CollectFeedbackAsync(feedbackText, ipAddress, userAgent);

            // Assert
            _mockSentimentAnalysisService.Verify(x => x.AnalyzeSentiment(feedbackText), Times.Once);
            _mockAnonymousFeedbackRepository.Verify(x => x.AddAsync(It.Is<AnonymousFeedback>(f => 
                f.FeedbackText == feedbackText &&
                f.Sentiment == expectedSentiment &&
                f.IpAddress == ipAddress &&
                f.UserAgent == userAgent
            )), Times.Once);
        }

        [Fact]
        public async Task GetAllFeedbacksAsync_ShouldReturnAllFeedbacks()
        {
            // Arrange
            var expectedFeedbacks = new List<AnonymousFeedback>
            {
                new AnonymousFeedback { Id = 1, FeedbackText = "Feedback 1", Sentiment = "Positive" },
                new AnonymousFeedback { Id = 2, FeedbackText = "Feedback 2", Sentiment = "Negative" }
            };
            
            _mockAnonymousFeedbackRepository
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(expectedFeedbacks);

            // Act
            var result = await _anonymousFeedbackService.GetAllFeedbacksAsync();

            // Assert
            Assert.Equal(expectedFeedbacks, result);
            _mockAnonymousFeedbackRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetFeedbacksBySentimentAsync_ShouldReturnFilteredFeedbacks()
        {
            // Arrange
            var sentiment = "Positive";
            var expectedFeedbacks = new List<AnonymousFeedback>
            {
                new AnonymousFeedback { Id = 1, FeedbackText = "Feedback 1", Sentiment = sentiment }
            };
            
            _mockAnonymousFeedbackRepository
                .Setup(x => x.GetBySentimentAsync(sentiment))
                .ReturnsAsync(expectedFeedbacks);

            // Act
            var result = await _anonymousFeedbackService.GetFeedbacksBySentimentAsync(sentiment);

            // Assert
            Assert.Equal(expectedFeedbacks, result);
            _mockAnonymousFeedbackRepository.Verify(x => x.GetBySentimentAsync(sentiment), Times.Once);
        }
    }
}