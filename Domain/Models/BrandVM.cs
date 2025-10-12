using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
//using Infrastructure.Localization;

namespace Domain.Models
{
    public class BrandVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string? BrandName { get; set; }
        [Required(ErrorMessage = "Name arabic is required.")]
        public string? BrandNameAr { get; set; }
        public List<IFormFile>? ImagesFormFiles { get; set; }
        //[LocalizedRequired("BrandName_Required")]
        public string? Description { get; set; }
        public string? CreatedDate { get; set; }
        public List<int?> CategoryIds { get; set; }
        public List<SelectListItem>? CategoryList { get; set; }
        public List<string>? ListOfRetrievedImages { get; set; } = new List<string>();
        public List<string>? OldImagesBytes { get; set; } = new List<string>();
        public string? DisplayBrandName
        {
            get
            {
                var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                return lang == "ar" ? BrandNameAr : BrandName ;
            }
        }
    }
}
