using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Product : BaseEntity
    {
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? OtherShopsPrice { get; set; }
        public decimal? DifferencePercentage { get; set; }
        public decimal? MaximumDiscountPercentage { get; set; }
        public decimal? BuyingPrice { get; set; }
        public int? StockQuantity{ get; set; }
        public int? StatusId{ get; set; }
        public DateOnly? ProductExpiryDate { get; set; }
        public string? ProductTags { get; set; }

        public int? CategoryId { get; set; } // Foreign Key
        public Category? Category { get; set; } // Navigation property
        public int? SupplierId { get; set; } // Foreign Key
        public Supplier? Supplier { get; set; } // Navigation property

        public ICollection<OrderItem>? OrderItems { get; set; } // Navigation property
        public ICollection<InventoryLog>? InventoryLogs { get; set; } // Navigation property
        public ICollection<Image>? Images { get; set; } // Navigation property

    }
}
