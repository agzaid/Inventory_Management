using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Image : BaseEntity
    {
        [MaxLength(600)]
        public string? ImageName { get; set; }
        [MaxLength(800)]
        public string? FilePath { get; set; }
        public byte[]? ImageByteArray { get; set; }
        public int? ViewingOrder { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }

        public int? SellerId { get; set; }
        public Seller? Seller { get; set; }
        public int? FeedbackId { get; set; }
        public Feedback? Feedback { get; set; }
    }
}