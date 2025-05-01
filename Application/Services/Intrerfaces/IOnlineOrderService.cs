using Domain.Entities;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Application.Services.Intrerfaces
{
    public interface IOnlineOrderService
    {
        Task<Result<List<ProductVM>>> GetProductsByCategory(int categoryId);
        Task<List<SelectListItem>> ShippingFreightSelectList();
        Task<List<DeliverySlotVM>> DeliverySlot();
        InvoiceVM CreateInvoiceForViewing(string orderNum);
        Task<Result<string>> CreateOrder(CartVM cart);
        PortalVM GetAllProductsForPortal();
        ProductVM GetProductDetails(int id);
        ProductVM GetProductById(int id);
        Result<List<OnlineOrderVM>> GetAllOrdersToBeInvoiced();
        Result<InvoiceVM> GetInvoiceForSpecificOnlineOrder(int id);
    }
}
