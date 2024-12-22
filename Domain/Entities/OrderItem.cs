using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; } // Foreign Key
        public Order? Order { get; set; } // Navigation property

        public int ProductId { get; set; } // Foreign Key
        public Product? Product { get; set; } // Navigation property

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
