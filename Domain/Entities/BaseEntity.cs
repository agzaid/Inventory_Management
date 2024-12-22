using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime? Create_Date { get; set; } = DateTime.UtcNow;
        public DateTime? Modified_Date { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
