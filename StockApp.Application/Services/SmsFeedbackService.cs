using StockApp.Application.Interfaces;
using StockApp.Domain.Entities;
using StockApp.Domain.Interfaces;

namespace StockApp.Application.Services
{
    public class SmsFeedbackService : ISmsFeedbackService
    {
        private readonly ISmsService _smsService;
        private readonly IFeedbackRepository _feedbackRepository;

        public SmsFeedbackService(ISmsService smsService, IFeedbackRepository feedbackRepository)
        {
            _smsService = smsService;
            _feedbackRepository = feedbackRepository;
        }
        public async Task SubmitFeedbackAsync(string userId, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                var feedback = new Feedback
                {
                    UserId = userId,
                    FeedbackText = message,
                    CreatedAt = DateTime.UtcNow,
                    Sentiment = null
                };

                await _feedbackRepository.SaveAsync(feedback);
                
            }
        }
        public async Task CollectFeedbackAsync(string phoneNumber, string feedback)
        {
            if (string.IsNullOrEmpty(feedback))
            {
                var message = "Olá! Por favor, envie seu feedback respondendo a esta mensagem.";
                await _smsService.SendSmsAsync(phoneNumber, message);
            }
            else
            {
                var fb = new Feedback
                {
                    UserId = phoneNumber,
                    FeedbackText = feedback,
                    CreatedAt = DateTime.UtcNow,
                    Sentiment = null
                };

                await _feedbackRepository.SaveAsync(fb);
                

                var thankYouMessage = "Obrigado pelo seu feedback!";
                await _smsService.SendSmsAsync(phoneNumber, thankYouMessage);
            }
        }
    }
}
