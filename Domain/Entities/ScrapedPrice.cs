using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class ScrapedPrice : BaseEntity
    {
        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Required]
        [StringLength(500)]
        public string SourceUrl { get; set; }

        [Required]
        public decimal Price { get; set; }

        [StringLength(10)]
        public string Currency { get; set; } = "EGP";

        [StringLength(50)]
        public string ScraperMethod { get; set; } // "Puppeteer" or "Selenium"

        public bool IsSuccessful { get; set; }

        [StringLength(500)]
        public string ErrorMessage { get; set; }

        public DateTime ScrapedDateTime { get; set; } = DateTime.UtcNow;

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
