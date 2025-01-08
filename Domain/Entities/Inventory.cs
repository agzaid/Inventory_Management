using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Inventory : BaseEntity
    {
        public int? QuantityInStock { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? OtherShopsPrice { get; set; }
        public decimal? DifferencePercentage { get; set; }
        public decimal? MaximumDiscountPercentage { get; set; }
        public decimal? BuyingPrice { get; set; }
        public DateOnly? ProductExpiryDate { get; set; }
        public string? Barcode { get; set; }
        //foreign keys
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
