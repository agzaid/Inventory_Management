using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Product : BaseEntity
    {
        [MaxLength(200)]
        public string? ProductName { get; set; }
        [MaxLength(200)]
        public string? ProductNameAr { get; set; }
        public string? Description { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? OtherShopsPrice { get; set; }
        public decimal? DifferencePercentage { get; set; }
        public decimal? MaximumDiscountPercentage { get; set; }
        public decimal? BuyingPrice { get; set; }
        public int? StockQuantity{ get; set; }
        public DateOnly? ProductExpiryDate { get; set; }
        public string? Barcode { get; set; }
        public int? StatusId{ get; set; }
        [MaxLength(900)]
        public string? ProductTags { get; set; }

        public int? CategoryId { get; set; } // Foreign Key
        public Category? Category { get; set; } // Navigation property

        public int? BrandId { get; set; } // Foreign Key
        public Brand? Brand { get; set; } // Navigation property

        public int? SupplierId { get; set; } // Foreign Key
        public Supplier? Supplier { get; set; } // Navigation property
        //public ICollection<Inventory>? Inventory { get; set; } // Navigation property

        public ICollection<InvoiceItem>? InvoiceItems { get; set; } // Navigation property
        public ICollection<InventoryLog>? InventoryLogs { get; set; } // Navigation property
        public ICollection<Image>? Images { get; set; } // Navigation property

    }
}
