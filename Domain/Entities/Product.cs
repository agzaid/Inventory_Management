using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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

        [MaxLength(1000)]
        public string? Description { get; set; }
        public bool IsKilogram { get; set; } = false;
        public decimal? PricePerGram { get; set; }

        public decimal? SellingPrice { get; set; }
        public decimal? OtherShopsPrice { get; set; }
        public decimal? DifferencePercentage { get; set; }
        public decimal? MaximumDiscountPercentage { get; set; }
        public decimal? BuyingPrice { get; set; }
        public decimal? StockQuantity{ get; set; }
        public DateOnly? ProductExpiryDate { get; set; }

        [MaxLength(1000)]
        public string? Barcode { get; set; }
        public int? StatusId{ get; set; }
        [MaxLength(900)]
        public string? ProductTags { get; set; }

        public int? CategoryId { get; set; } // Foreign Key
        public Category? Category { get; set; } // Navigation property

        public int? BrandId { get; set; } // Foreign Key
        public Brand? Brand { get; set; } // Navigation property

        public int? SellerId { get; set; } // Foreign Key
        public Seller? Seller { get; set; } // Navigation property
        //public ICollection<Inventory>? Inventory { get; set; } // Navigation property

        public ICollection<InvoiceItem>? InvoiceItems { get; set; } // Navigation property
        public ICollection<InventoryLog>? InventoryLogs { get; set; } // Navigation property
        public ICollection<Image>? Images { get; set; } // Navigation property


        public string? DisplayProductName
        {
            get
            {
                var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                return lang == "ar" ? ProductNameAr : ProductName;
            }
        }
    }
}
