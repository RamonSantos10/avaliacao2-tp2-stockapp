namespace StockApp.Domain.Entities
{
    public class Feedback
    {
        public int Id { get; set; } 
        public string UserId { get; set; }
        public string FeedbackText { get; set; }
        public string Sentiment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
