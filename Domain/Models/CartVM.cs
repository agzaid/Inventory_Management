namespace Domain.Models
{
    public class CartVM
    {
        public double? TotalPrice { get; set; }
        public double? PriceBeforeShipping { get; set; }
        public string? Shipping { get; set; }
        public string? Location { get; set; }
        public List<ItemsVM> ItemsVMs { get; set; } = new();
    }
    public class ItemsVM
    {
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductPrice { get; set; }
        public int Quantity { get; set; }
    }
}
