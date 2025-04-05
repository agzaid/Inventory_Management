
using System.Web.Mvc;

namespace Domain.Models
{
    public class CartVM
    {
        public double? TotalPrice { get; set; }
        public double? PriceBeforeShipping { get; set; }
        public string? ShippingAreaPrice { get; set; }
        public string? ShippingAreaName { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerAddress { get; set; }
        public string? CustomerPhone { get; set; }
        public string[]? SelectedSlots { get; set; }
        public string? Location { get; set; }
        public List<ItemsVM> ItemsVMs { get; set; } = new();
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
