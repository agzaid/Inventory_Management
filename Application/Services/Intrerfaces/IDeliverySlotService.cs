using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Intrerfaces
{
    public interface IDeliverySlotService
    {
        IEnumerable<DeliverySlotVM> GetAllDeliverySlot();
        DeliverySlotVM GetDeliveryById(int id);
        Task<string> CreateDeliverySlot(DeliverySlotVM delivery);
        Task<bool> UpdateDeliverySlot(DeliverySlotVM delivery);
        Task<bool> DeleteDeliverySlot(int id);
    }
}
