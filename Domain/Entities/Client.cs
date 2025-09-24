namespace Domain.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }

        // Navigation property 
        public ICollection<Sale>? Sales { get; set; }

    }
}
