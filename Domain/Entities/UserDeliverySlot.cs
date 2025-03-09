using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserDeliverySlot : BaseEntity
    {
        public int UserId { get; set; }
        public int DeliverySlotId { get; set; }
        public DeliverySlot? DeliverySlot { get; set; }
        //public User User { get; set; }
    }
}
