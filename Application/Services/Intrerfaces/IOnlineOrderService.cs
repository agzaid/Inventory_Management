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
        Task<Result<List<ProductVM>>> GetProductsByCategory(int? categoryId);
        Task<Result<List<ProductVM>>> GetProductsByName(string? name);
        Task<List<SelectListItem>> ShippingFreightSelectList();
        Task<List<DeliverySlotVM>> DeliverySlot();
        InvoiceVM CreateInvoiceForViewing(string orderNum);
        Task<Result<string>> CreateOrder(CartVM cart);
        //Task<Result<string>> CreateFeedback(FeedbackVM feedback);
        PortalVM GetAllProductsForPortal();
        ProductVM GetProductDetails(int id);
        ProductVM GetProductById(int id);
        Result<List<OnlineOrderVM>> GetAllOrdersToBeInvoiced();
        Result<InvoiceVM> GetInvoiceForSpecificOnlineOrder(int id);
        Task<Result<PaginatedResult<ProductVM>>> GetProductsPaginated(int pageNumber, int pageSize, int? categoryId);
        Task<Result<bool>> UpdateOrderStatus(string orderNum, string options);
        Task<List<SelectListItem>> DistrictSelectList();
        Result<List<OnlineOrderVM>> GetAllOrdersPending();
    }
}
