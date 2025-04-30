using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserDeliverySlot : BaseEntity
    {
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int DeliverySlotId { get; set; }
        public DeliverySlot? DeliverySlot { get; set; }

        public int OnlineOrderId { get; set; }
        public OnlineOrder? OnlineOrder { get; set; }
        //public User User { get; set; }
    }
}
