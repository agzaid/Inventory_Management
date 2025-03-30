
using System.Web.Mvc;

namespace Domain.Models
{
    public class DistrictVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public string? CreatedDate { get; set; }
        public int? AreaId { get; set; }
        public ICollection<SelectListItem> Areas { get; set; }
    }
}
