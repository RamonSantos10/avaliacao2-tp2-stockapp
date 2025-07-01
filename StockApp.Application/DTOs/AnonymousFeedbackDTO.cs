using System.ComponentModel.DataAnnotations;

namespace StockApp.Application.DTOs
{
    public class AnonymousFeedbackDTO
    {
        [Required(ErrorMessage = "O feedback é obrigatório")]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "O feedback deve ter entre 5 e 1000 caracteres")]
        public string Feedback { get; set; }
    }
    
    public class AnonymousFeedbackResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string? SentimentAnalysis { get; set; }
    }
}