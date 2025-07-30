using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [MaxLength(100)]
        public string? ActionType { get; set; } // e.g., "Add", "Remove"

        [MaxLength(1000)]
        public string? Reason { get; set; }
    }
}
