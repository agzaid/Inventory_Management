namespace Domain.Models
{
    public class InvoiceItemVM
    {
        private string? productName;

        public int Id { get; set; }
        public string? ProductName
        {
            get { return productName; }
            set { productName = value.ToLower(); }
        }
        public double? IndividualDiscount { get; set; }
        public int? Quantity { get; set; }
        public decimal? PriceSoldToCustomer { get; set; }
        public double? ShippingPrice { get; set; }

        public int? StockQuantityFromProduct { get; set; }
        public decimal? DifferencePercentageFromProduct { get; set; }
        public decimal? BuyingPriceFromProduct { get; set; }
        public decimal? MaximumDiscountPercentageFromProduct { get; set; }
        public decimal? SellingPriceFromProduct { get; set; }
        public decimal? OtherShopsPriceFromProduct { get; set; }
        public DateOnly? ProductExpiryDateFromProduct { get; set; }
        public string? ProductTagsFromProduct { get; set; }
        public string? BarcodeFromProduct { get; set; }
        public string? CustomerName { get; set; }
        public string? MobileNumber { get; set; }

    }
}
