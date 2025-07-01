namespace StockApp.Domain.Entities
{
    public class Review
    {
        public int Id { get; set; } 
        public int ProductId { get; set; } 
        public string UserId { get; set; } 
        public int Rating { get; set; } 
        public string Comment { get; set; } 
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Sentiment { get; set; }

    }
}
