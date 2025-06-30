using Moq;
using System.Threading.Tasks;
using Xunit;
using StockApp.Application.Services;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;

namespace StockApp.Domain.Test
{
    public class SmsFeedbackServiceTests
    {
        [Fact]
        public async Task SubmitFeedbackAsync_ShouldSaveFeedback_WhenFeedbackIsNotEmpty()
        {
            // Arrange
            var mockSmsService = new Mock<ISmsService>();
            var mockFeedbackRepository = new Mock<IFeedbackRepository>();

            var smsFeedbackService = new SmsFeedbackService(mockSmsService.Object, mockFeedbackRepository.Object);

            string phoneNumber = "+5511999990000";
            string feedbackText = "Ótimo serviço!";

            // Act
            await smsFeedbackService.SubmitFeedbackAsync(phoneNumber, feedbackText);

            // Assert
            mockFeedbackRepository.Verify(
                repo => repo.SaveAsync(It.Is<Feedback>(f =>
                    f.UserId == phoneNumber && f.FeedbackText == feedbackText)),
                Times.Once);
        }
    }
}
