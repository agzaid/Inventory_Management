using Domain.Enums;

namespace Domain.Models
{
    public class OnlineOrderVM
    {
        public int Id { get; set; }
        public string? OrderNumber { get; set; }
        public string? CustomerName { get; set; }
        public string? OrderDate { get; set; }
        public double? GrandTotalAmount { get; set; }
        public string? DeliverySlots { get; set; }
        public string? Area { get; set; }
        public string? OrderStatus { get; set; }
        public Status Status { get; set; }

    }
}



