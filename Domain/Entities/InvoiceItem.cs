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
        public decimal? IndividualDiscount { get; set; }
        public decimal? Quantity { get; set; }
        // this field is for SellingPriceFromProduct but with discount applied from seller like me
        public decimal? PriceSoldToCustomer { get; set; }
        public decimal? ShippingPrice { get; set; }

        public decimal? StockQuantityFromProduct { get; set; }
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
