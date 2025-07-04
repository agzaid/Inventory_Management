using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class BrandVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string? BrandName { get; set; }
        [Required(ErrorMessage = "Name arabic is required.")]
        public string? BrandNameAr { get; set; }
        public string? Description { get; set; }
        public string? FormToken { get; set; }
        public string? CreatedDate { get; set; }
    }
}
