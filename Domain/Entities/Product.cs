namespace Domain.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public required string Name { get; set; }
        public required decimal Price { get; set; }
        public required int StockQuantity { get; set; }
        public required string Category { get; set; }

        // Navigation property
        public ICollection<SaleDetails>? SaleDetails { get; set; }
    }
}

