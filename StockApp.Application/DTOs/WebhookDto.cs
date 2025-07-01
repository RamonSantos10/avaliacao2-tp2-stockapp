using System.ComponentModel.DataAnnotations;

namespace StockApp.Application.DTOs
{
    public class WebhookDto
    {
        [Required]
        public string Event { get; set; } = string.Empty;
        
        [Required]
        public string Url { get; set; } = string.Empty;
        
        public object? Data { get; set; }
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        public string? Secret { get; set; }
        
        public Dictionary<string, string>? Headers { get; set; }
    }
    
    public class WebhookResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}