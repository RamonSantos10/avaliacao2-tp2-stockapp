namespace StockApp.Domain.Entities
{
    public class AnonymousFeedback
    {
        public int Id { get; set; }
        public string FeedbackText { get; set; }
        public string Sentiment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}