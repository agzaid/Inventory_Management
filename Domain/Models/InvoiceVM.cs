using Domain.Enums;
using System.Web.Mvc;

namespace Domain.Models
{
    public class InvoiceVM
    {
        public InvoiceVM()
        {
            //ListOfRegoins = Enum.GetNames(typeof(Regoin))
            //  .Select(v => new SelectListItem
            //  {
            //      Text = v,
            //      Value = v
            //  }).ToList();
        }
        public int Id { get; set; }
        public List<string>? productInput { get; set; } = new List<string>();
        public List<string>? priceInput { get; set; } = new List<string>();
        public List<string>? stockInput { get; set; } = new List<string>();
        public List<string>? quantityInput { get; set; } = new List<string>();
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ShippingNotes { get; set; }
        public string? Number { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? AreaId { get; set; }
        public List<SelectListItem>? ListOfAreas { get; set; }


        public int? ProductId { get; set; }
    }
}
