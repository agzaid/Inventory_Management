using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Domain.Models
{
    public class ProductVM
    {
        public ProductVM()
        {
            ListOfStatus = Enum.GetNames(typeof(Status))
              .Select(v => new SelectListItem
              {
                  Text = v,
                  Value = v
              }).ToList();
        }
        public int? Id { get; set; }
        [Required(ErrorMessage = "Product Name is required.")]
        public string? ProductName { get; set; }
        [Required(ErrorMessage = "Product Name arabic is required.")]
        public string? ProductNameAr { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }
        public string? Brand { get; set; }
        public string? Seller { get; set; }
        public string? Barcode { get; set; }
        public List<IFormFile>? ImagesFormFiles { get; set; }
        [Required(ErrorMessage = "Selling Price is required.")]
        public decimal? SellingPrice { get; set; }
        public bool IsKilogram { get; set; } = false;
        public decimal? PricePerGram { get; set; }
        public decimal? PurchasedGrams { get; set; }
        public decimal? TotalPurchasedPricePerGrams { get; set; }
        public decimal? OtherShopsPrice { get; set; }
        public string? DifferencePercentage { get; set; }
        public string? MaximumDiscountPercentage { get; set; }
        public decimal? DiscPerceForCreateInvoice { get; set; }
        public decimal? BuyingPrice{ get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        public decimal? StockQuantity { get; set; }
        public string? CreatedDate { get; set; }
        public string? ExpiryDate { get; set; }
        public Status? ProductStatus { get; set; }

        [Required(ErrorMessage = "Tags is required.")]
        public string? ProductTags { get; set; }
        public int? InputQuantity { get; set; }
        public string? StatusId{ get; set; }
        public string? CategoryId{ get; set; }

        [Required(ErrorMessage = "Brand is required.")]
        public string? BrandId{ get; set; }

        [Required(ErrorMessage = "Seller is required.")]
        public string? SellerId{ get; set; }
        public string? CategoryName{ get; set; }
        public List<string>? OldImagesBytes { get; set; } = new List<string>();
        public List<string>? ListOfRetrievedImages { get; set; } = new List<string>();
        public List<SelectListItem>? ListOfStatus { get; set; }
        public List<SelectListItem>? ListOfCategory { get; set; }
        public List<SelectListItem>? ListOfBrands { get; set; }
        public List<SelectListItem>? ListOfSellers { get; set; }

        public string? DisplayProductName
        {
            get
            {
                var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                return lang == "ar" ? ProductNameAr : ProductName;
            }
        }
    }
}
