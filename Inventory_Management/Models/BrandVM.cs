using Infrastructure.Localization;

namespace Inventory_Management.Models
{
    public class BrandVM
    {
        public int Id { get; set; }
        [LocalizedRequired("BrandName_Required")]

        public string? BrandName { get; set; }
        [LocalizedRequired("BrandName_Required")]

        public string? BrandNameAr { get; set; }
        public List<IFormFile>? ImagesFormFiles { get; set; }
        [LocalizedRequired("BrandName_Required")]
        public string? Description { get; set; }
        public byte[]? RowVersion { get; set; }
        public string? FormToken { get; set; }
        public string? CreatedDate { get; set; }
        public List<string>? ListOfRetrievedImages { get; set; } = new List<string>();
        public List<string>? OldImagesBytes { get; set; } = new List<string>();
    }
}
