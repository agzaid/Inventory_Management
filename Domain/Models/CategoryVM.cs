using System.ComponentModel.DataAnnotations;
using System.Globalization;

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

        public string? DisplayCategoryName
        {
            get
            {
                var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                return lang == "ar" ? CategoryNameAr : CategoryName;
            }
        }
    }
}
