
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Domain.Models
{
    public class CartVM
    {
        public string? OrderNumber { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? PriceBeforeShipping { get; set; }
        public string? ShippingAreaPrice { get; set; }
        public string? ShippingAreaName { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string? CustomerAddress { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits (only numbers allowed).")]
        [Required(ErrorMessage = "Phone is required.")]
        public string? CustomerPhone { get; set; }
        public string? OptionalCustomerPhone { get; set; }
        public string? StreetName { get; set; }
        //[LocalizedRequired("CustomerName_Required")]
        public string? BuildingNumber { get; set; }
        //[LocalizedRequired("CustomerName_Required")]
        public string? Floor { get; set; }
        //[LocalizedRequired("CustomerName_Required")]
        public string? ApartmentNumber { get; set; }
        //[LocalizedRequired("CustomerName_Required")]
        public string? LandMark { get; set; }
        public string? Location { get; set; }
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
        public decimal? ProductPrice { get; set; }
        public decimal Quantity { get; set; }
    }
}
