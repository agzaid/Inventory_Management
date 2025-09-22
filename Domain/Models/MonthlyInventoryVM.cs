using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class MonthlyInventoryVM
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal RemainingStock { get; set; }

        // New fields for filtering
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}