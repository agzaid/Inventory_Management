using System.Web.Mvc;

namespace Domain.Models
{
    public class InvoiceVM
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public string? Number { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? StatusId { get; set; }
        public List<SelectListItem>? ListOfStatus { get; set; }


        public int? ProductId { get; set; }
    }
}
