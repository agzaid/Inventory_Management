using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class MonthlyInventoryFilterVM
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<MonthlyInventoryVM> Report { get; set; }
    }
}
