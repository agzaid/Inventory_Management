using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Seller : BaseEntity
    {

        [MaxLength(500)]
        public string? SellerName { get; set; }
        public string? Description { get; set; }
        public string? SellerNameAr { get; set; }


        [MaxLength(1000)]
        public string? Address { get; set; }
        public int? PhoneNumber { get; set; }

        public ICollection<Product>? Products { get; set; }
        public ICollection<Image>? Images { get; set; } // Navigation property
    }
}
