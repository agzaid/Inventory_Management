using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DeliverySlot : BaseEntity
    {
        [MaxLength(100)]
        public string? StartTime { get; set; }
        [MaxLength(100)]
        public string? EndTime { get; set; }
        [MaxLength(100)]
        public string? AM_PM { get; set; }
        public bool IsAvailable { get; set; } = true;

        
        public ICollection<UserDeliverySlot>? UserDeliverySlots { get; set; }
        //public ICollection<OnlineOrder>? OnlineOrders { get; set; }
    }
}
