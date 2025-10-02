using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Category : BaseEntity
    {
        [MaxLength(100)]
        public string? CategoryName { get; set; }
        [MaxLength(100)]
        public string? CategoryNameAr { get; set; }
        [MaxLength(800)]
        public string? Description { get; set; }

        public ICollection<Product>? Products { get; set; }


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
