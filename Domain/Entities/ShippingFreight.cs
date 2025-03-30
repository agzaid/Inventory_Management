using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ShippingFreight : BaseEntity
    {
        //Area > District 
        public string? Area { get; set; }
        //public string? District { get; set; }
        public double? Price { get; set; }

        public ICollection<District>? Districts { get; set; }

        // public ICollection<Product>? Products { get; set; }
    }
}
