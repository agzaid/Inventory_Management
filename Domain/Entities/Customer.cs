using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Area { get; set; }
        public string? Address { get; set; }

        public ICollection<Invoice>? Invoices { get; set; } // Navigation property
    }
}
