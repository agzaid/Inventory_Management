using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IProductRepository Product { get; }
        ICategoryRepository Category { get; }
        IImageRepository Image { get; }
        IInvoiceRepository Invoice { get; }
        IShippingFreightRepository ShippingFreight { get; }
        ICustomerRepository Customer { get; }
        IDeliverySlotRepository DeliverySlot { get; }
        IDistrictRepository District { get; }
        IOnlineOrderRepository OnlineOrder { get; }
        IBrandRepository Brand { get; }
        Task SaveAsync();

    }
}
