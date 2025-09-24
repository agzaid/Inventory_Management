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
        public string? SupplierName { get; set; }

        [MaxLength(1000)]
        public string? Address { get; set; }
        public int? PhoneNumber { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
