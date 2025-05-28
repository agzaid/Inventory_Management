using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class CategoryVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string? CategoryName { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string? CategoryNameAr { get; set; }
        public string? Description { get; set; }
        public string? FormToken { get; set; }
        public string? CreatedDate { get; set; }
    }
}
