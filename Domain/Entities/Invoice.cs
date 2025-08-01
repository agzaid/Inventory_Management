﻿using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Invoice : BaseEntity
    {
        public Invoice()
        {
            InvoiceNumber = Generate12DigitSerialNumber();
        }
        [MaxLength(200)]
        public string? InvoiceNumber { get;  set; }

        [MaxLength(2000)]
        public string? AllProductItems { get; set; }
        public decimal? ProductsOnlyAmount { get; set; }
        public double? GrandTotalAmount { get; set; }
        public decimal? AllDiscountInput { get; set; }

        [MaxLength(1000)]
        public string? ShippingNotes { get; set; }
        public double? ShippingPrice { get; set; }

        public int? AreaId { get; set; }
        public int? CustomerId { get; set; } // Foreign Key
        public Customer? Customer { get; set; } // Navigation property

        public DateTime OrderDate { get; set; }

        public OnlineOrder OnlineOrder { get; set; }

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
