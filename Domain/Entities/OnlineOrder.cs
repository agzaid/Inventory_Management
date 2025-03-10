﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OnlineOrder : BaseEntity
    {
        public OnlineOrder()
        {
            OrderNumber = Generate12DigitSerialNumber();
        }
        public string? OrderNumber { get; private set; }
        public string? AllProductItems { get; set; }
        public decimal? ProductsOnlyAmount { get; set; }
        public double? GrandTotalAmount { get; set; }
        public decimal? AllDiscountInput { get; set; }
        public string? ShippingNotes { get; set; }
        public double? ShippingPrice { get; set; }

        public int? AreaId { get; set; }
        public int? CustomerId { get; set; } // Foreign Key
        public Customer? Customer { get; set; } // Navigation property

        public DateTime OrderDate { get; set; }

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
