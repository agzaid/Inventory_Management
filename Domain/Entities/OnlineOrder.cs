using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OnlineOrder : BaseEntity
    {
        //public OnlineOrder()
        //{
        //  OrderNumber = Generate12DigitSerialNumber();
        //}
        [MaxLength(100)]
        public string? OrderNumber { get; set; }
        [MaxLength(500)]
        public string? IndividualProductsNames { get; set; }
        [MaxLength(100)]
        public string? IndividualProductsPrices { get; set; }
        [MaxLength(100)]
        public string? IndividualProductsQuatities { get; set; }
        public decimal? GrandTotalAmount { get; set; }
        public decimal? AmountBeforeShipping{ get; set; }
        public decimal? AllDiscountInput { get; set; }
        [MaxLength(1000)]
        public string? Address { get; set; }
        public decimal? ShippingPrice { get; set; }

        [MaxLength(200)]
        public string? StreetName { get; set; }

        [MaxLength(100)]
        public string? BuildingNumber { get; set; }

        [MaxLength(100)]
        public string? Floor { get; set; }

        [MaxLength(100)]
        public string? ApartmentNumber { get; set; }

        [MaxLength(1000)]
        public string? LandMark { get; set; }

        [MaxLength(1000)]
        public string? Location { get; set; }
        public string[]? DeliverySlotsAsString { get; set; }

        public int? AreaId { get; set; }
        public int? CustomerId { get; set; } // Foreign Key
        public Customer? Customer { get; set; } // Navigation property

        public DateTime OrderDate { get; set; }
        public Status OrderStatus { get; set; } = Status.InProgress;
        public int? InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }

        public ICollection<UserDeliverySlot> UserDeliverySlots { get; set; }
        public List<InvoiceItem>? InvoiceItems { get; set; } = new List<InvoiceItem>();


        private string Generate12DigitSerialNumber()
        {
            Guid guid = Guid.NewGuid();
            string guidString = guid.ToString("N");
            string numericPart = guidString.Substring(0, 12);
            return numericPart;
        }
    }
}
