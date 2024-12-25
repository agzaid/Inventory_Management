namespace Domain.Entities
{
    public class Image : BaseEntity
    {
        public string? ImageName { get; set; }
        public string? FilePath { get; set; }
        public byte[]? ImageByteArray { get; set; }
        public int? ViewingOrder { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }
    }
}