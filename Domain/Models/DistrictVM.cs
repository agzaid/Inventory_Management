
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Domain.Models
{
    public class DistrictVM
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? NameAr { get; set; }
        [Required]
        public double? Price { get; set; }
        public string? CreatedDate { get; set; }
        public string? AreaName { get; set; }
        public int? AreaId { get; set; }
        public ICollection<SelectListItem> Areas { get; set; } = new List<SelectListItem>();
    }
}
