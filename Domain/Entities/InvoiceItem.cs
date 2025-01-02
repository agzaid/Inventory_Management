using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class InvoiceItem : BaseEntity
    {
        public int InvoiceId { get; set; } // Foreign Key
        public Invoice? Invoice{ get; set; } // Navigation property

        public int ProductId { get; set; } // Foreign Key
        public Product? Product { get; set; } // Navigation property

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
