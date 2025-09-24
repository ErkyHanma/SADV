namespace Domain.Entities
{
    public class SaleDetails
    {
        public int SaleDetailId { get; set; }
        public int ProductId { get; set; }
        public int SaleId { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }

        // Navigation properties
        public Product? Product { get; set; }
        public Sale? Sale { get; set; }
    }
}


