using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class District : BaseEntity
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(100)]
        public string? NameAr { get; set; }
        public decimal? Price { get; set; }

        public int? ShippingFreightId { get; set; }
        public ShippingFreight? ShippingFreight { get; set; }
    }
}
