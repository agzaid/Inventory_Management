using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class DeliverySlotVM
    {
        public int Id { get; set; }
        [Required]
        public string? StartTime{ get; set; }
        [Required]
        public string? EndTime{ get; set; }
        [Required]
        public string? AM_PM { get; set; }
        public bool IsAvailable { get; set; }
        public string? CreatedDate { get; set; }
    }
}
