using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Customer : BaseEntity
    {
        [MaxLength(200)]
        public string? CustomerName { get; set; }
        [MaxLength(200)]
        public string? CustomerNameAr { get; set; }
        [MaxLength(100)]
        public string? Email { get; set; }
        [MaxLength(100)]
        public string? Phone { get; set; }
        [MaxLength(100)]
        public string? Area { get; set; }
        [MaxLength(500)]
        public string? Address { get; set; }

        public ICollection<Invoice>? Invoices { get; set; } // Navigation property
        public ICollection<UserDeliverySlot>? UserDeliverySlots { get; set; } // Navigation property
    }
}
