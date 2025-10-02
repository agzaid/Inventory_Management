using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
//using Infrastructure.Localization;

namespace Domain.Models
{
    public class SellerVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string? SellerName { get; set; }
        [Required(ErrorMessage = "Name arabic is required.")]
        public string? SellerNameAr { get; set; }
        public List<IFormFile>? ImagesFormFiles { get; set; }
        //[LocalizedRequired("BrandName_Required")]
        public string? Description { get; set; }
        public string? Address { get; set; }
        public int? PhoneNumber { get; set; }
        public string? CreatedDate { get; set; }
        public List<string>? ListOfRetrievedImages { get; set; } = new List<string>();
        public List<string>? OldImagesBytes { get; set; } = new List<string>();
    }
}
