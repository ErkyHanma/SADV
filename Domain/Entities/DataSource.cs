namespace Domain.Entities
{
    public class DataSource
    {
        public int DataSourceId { get; set; }
        public required string SourceType { get; set; }
        public DateTime LoadDate { get; set; } = DateTime.UtcNow;
    }
}
