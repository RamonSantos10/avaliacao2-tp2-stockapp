namespace StockApp.Domain.Entities
{
    public class Feedback
    {
        public string Id { get; set; }= Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string Message { get; set; }
        public string Sentiment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
