using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ShippingFreight : BaseEntity
    {
        public string? Area { get; set; }
        public string? Region { get; set; }
        public double? Price { get; set; }

       // public ICollection<Product>? Products { get; set; }
    }
}
