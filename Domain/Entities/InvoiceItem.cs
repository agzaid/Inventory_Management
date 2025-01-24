using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class InvoiceItem : BaseEntity
    {
        private string? productName;

        // here you will put all other properties that are not there in other entities
        public string? ProductName
        {
            get { return productName; }
            set { productName = value.ToLower(); }
        }
        public double? IndividualDiscount { get; set; }
        public int? Quantity { get; set; }
        public double? ShippingPrice { get; set; }

        public int? InvoiceId { get; set; } // Foreign Key
        public Invoice? Invoice { get; set; } // Navigation property

        public int? ProductId { get; set; } // Foreign Key
        public Product? Product { get; set; } // Navigation property

        public int? AreaId { get; set; }
        public decimal? Price { get; set; }
    }
}
