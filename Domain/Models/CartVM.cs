
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Domain.Models
{
    public class CartVM
    {
        public double? TotalPrice { get; set; }
        public double? PriceBeforeShipping { get; set; }
        public string? ShippingAreaPrice { get; set; }
        public string? ShippingAreaName { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string? CustomerAddress { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits (only numbers allowed).")]
        [Required(ErrorMessage = "Phone is required.")]
        public string? CustomerPhone { get; set; }
        public string[]? SelectedSlots { get; set; }
        public string? Location { get; set; }
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
