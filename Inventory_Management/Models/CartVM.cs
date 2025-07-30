using Domain.Models;
using Infrastructure.Localization;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Inventory_Management.Models
{
    public class CartVM
    {
        public CartVM()
        {
            OrderNumber = Generate12DigitSerialNumber();
        }

        private string? Generate12DigitSerialNumber()
        {
            Guid guid = Guid.NewGuid();
            string guidString = guid.ToString("N");
            string numericPart = guidString.Substring(0, 12);
            return numericPart;
        }

        public string? OrderNumber { get; set; }
        public double? TotalPrice { get; set; }
        public double? PriceBeforeShipping { get; set; }
        public string? ShippingAreaPrice { get; set; }
        public string? ShippingAreaName { get; set; }

        //[Required(ErrorMessage = "Username is required.")]
        [LocalizedRequired("CustomerName_Required")]
        public string? CustomerName { get; set; }

        [LocalizedRequired("AddressName_Required")]
        public string? CustomerAddress { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits (only numbers allowed).")]
        [LocalizedRequired("PhoneNumber_Required")]
        public string? CustomerPhone { get; set; }

        //[LocalizedRequired("CustomerName_Required")]
        public string? StreetName { get; set; }
        //[LocalizedRequired("CustomerName_Required")]
        public string? BuildingNumber { get; set; }
        //[LocalizedRequired("CustomerName_Required")]
        public string? Floor { get; set; }
        //[LocalizedRequired("CustomerName_Required")]
        public string? ApartmentNumber { get; set; }
        //[LocalizedRequired("CustomerName_Required")]
        public string? LandMark { get; set; }
        public string? locationInput { get; set; }
        public string[]? SelectedSlots { get; set; }
        public List<ItemsVM> ItemsVMs { get; set; } = new();
        public List<DeliverySlotVM> DeliverySlotVMs { get; set; } = new();
        public int? AreaId { get; set; } = new();
        public List<SelectListItem> Areas { get; set; } = new();

    }
    public class ItemsVM
    {
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductPrice { get; set; }
        public int Quantity { get; set; }
    }
}
    
