namespace Domain.Entities
{
    public class Sale
    {
        public int SaleId { get; set; }
        public required int ClientId { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string? Status { get; set; }

        // Navigation property  
        public Client? Client { get; set; }
        public SaleDetails? SaleDetails { get; set; }
    }
}


