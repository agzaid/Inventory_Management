using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Brand is required.")]
        public string? Brand { get; set; }
        public string? Barcode { get; set; }
        public List<IFormFile>? ImagesFormFiles { get; set; }
        [Required(ErrorMessage = "Selling Price is required.")]
        public decimal? SellingPrice { get; set; }
        public decimal? OtherShopsPrice { get; set; }
        public string? DifferencePercentage { get; set; }
        public string? MaximumDiscountPercentage { get; set; }
        public decimal? DiscPerceForCreateInvoice { get; set; }
        public decimal? BuyingPrice{ get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        public int? StockQuantity { get; set; }
        public string? CreatedDate { get; set; }
        public string? ExpiryDate { get; set; }
        public Status? ProductStatus { get; set; }

        [Required(ErrorMessage = "Tags is required.")]
        public string? ProductTags { get; set; }
        public int? InputQuantity { get; set; }
        public string? StatusId{ get; set; }
        public string? CategoryId{ get; set; }
        public string? CategoryName{ get; set; }
        public List<string>? OldImagesBytes { get; set; } = new List<string>();
        public List<string>? ListOfRetrievedImages { get; set; } = new List<string>();
        public List<SelectListItem>? ListOfStatus { get; set; }
        public List<SelectListItem>? ListOfCategory { get; set; }


    }
}
