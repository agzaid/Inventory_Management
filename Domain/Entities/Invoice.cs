﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Invoice : BaseEntity
    {
        public int CustomerId { get; set; } // Foreign Key
        public Customer? Customer { get; set; } // Navigation property

        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public ICollection<InvoiceItem>? OrderItems { get; set; }
    }
}
