using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Supplier : BaseEntity
    {
        public string? SupplierName { get; set; }
        public string? Address { get; set; }
        public int? PhoneNumber { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
