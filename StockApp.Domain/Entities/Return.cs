using System.Text.Json.Serialization;

namespace StockApp.Domain.Entities
{
    public class Return
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string ReturnReason { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Status { get; set; } // "Pending", "Approved", "Rejected"

        [JsonIgnore]
        public Product Product { get; set; }
    }
}
