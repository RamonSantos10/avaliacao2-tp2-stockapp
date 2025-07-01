using System.ComponentModel.DataAnnotations;

namespace StockApp.Application.DTOs
{
    public class AnonymousFeedbackDTO
    {
        [Required(ErrorMessage = "O feedback é obrigatório")]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "O feedback deve ter entre 5 e 1000 caracteres")]
        public string Feedback { get; set; }
        
        public string? Category { get; set; } // Categoria opcional do feedback (ex: "Produto", "Atendimento", "Entrega")
        
        public int? Rating { get; set; } // Avaliação opcional de 1 a 5
    }
    
    public class AnonymousFeedbackResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string? SentimentAnalysis { get; set; }
    }
}