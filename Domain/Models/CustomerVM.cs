

using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Domain.Models
{
    public class CustomerVM
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        [Required]
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Area { get; set; }
        public string? Address { get; set; }
        public string? AreaId { get; set; }
        public string? CreatedDate { get; set; }
        public List<SelectListItem>? ListOfAreas { get; set; }
    }
}
