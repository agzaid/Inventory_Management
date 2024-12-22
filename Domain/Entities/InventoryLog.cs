using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class InventoryLog : BaseEntity
    {
        public int ProductId { get; set; } // Foreign Key
        public Product? Product { get; set; } // Navigation property

        public DateTime Date { get; set; }
        public int QuantityChanged { get; set; }
        public string? ActionType { get; set; } // e.g., "Add", "Remove"
        public string? Reason { get; set; }
    }
}
