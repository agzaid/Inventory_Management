namespace Domain.Models
{
    public class ShippingFreightVM
    {
        public int Id { get; set; }
        public string? Area { get; set; }
        public string[]? Districts { get; set; }
        public double? Price { get; set; }
        public string? CreatedDate { get; set; }
    }
}
