using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class InvoiceItem : BaseEntity
    {
        private string? productName;

        // here you will put all other properties that are not there in other entities
        [MaxLength(200)]
        public string? ProductName
        {
            get { return productName; }
            set { productName = value?.ToLower(); }
        }
        public double? IndividualDiscount { get; set; }
        public int? Quantity { get; set; }
        public decimal? PriceSoldToCustomer { get; set; }
        public double? ShippingPrice { get; set; }

        public int? StockQuantityFromProduct { get; set; }
        public decimal? DifferencePercentageFromProduct { get; set; }
        public decimal? BuyingPriceFromProduct { get; set; }
        public decimal? MaximumDiscountPercentageFromProduct { get; set; }
        public decimal? SellingPriceFromProduct { get; set; }
        public decimal? OtherShopsPriceFromProduct { get; set; }
        public DateOnly? ProductExpiryDateFromProduct { get; set; }

        [MaxLength(1000)]
        public string? ProductTagsFromProduct { get; set; }

        [MaxLength(1000)]
        public string? BarcodeFromProduct { get; set; }


        public int? InvoiceId { get; set; } // Foreign Key
        public Invoice? Invoice { get; set; } // Navigation property

        public int? ProductId { get; set; } // Foreign Key
        public Product? Product { get; set; } // Navigation property

    }
}
