namespace Domain.Models
{
    public class ImageVM
    {
        public int Id { get; set; }
        public string? ImageName { get; set; }
        public string? FilePath { get; set; }
        public int? ViewingOrder { get; set; }

        public int? ProductId { get; set; }
    }
}
