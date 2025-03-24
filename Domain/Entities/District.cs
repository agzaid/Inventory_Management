using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class District : BaseEntity
    {
        public string? Name { get; set; }
        public double? Price { get; set; }

        public int? ShippingFreightId { get; set; }
        public ShippingFreight? ShippingFreight { get; set; }
    }
}
