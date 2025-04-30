using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class DeliverySlot : BaseEntity
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string? AM_PM { get; set; }
        public bool IsAvailable { get; set; } = true;

        
        public ICollection<UserDeliverySlot>? UserDeliverySlots { get; set; }
        //public ICollection<OnlineOrder>? OnlineOrders { get; set; }
    }
}
