using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
//using Infrastructure.Localization;

namespace Domain.Models
{
    public class FeedbackVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
        public string? Phone { get; set; }
        public string? CreatedDate { get; set; }
        public List<IFormFile>? ImagesFormFiles { get; set; }
        public List<string>? retrievedImages { get; set; }

    }
}
