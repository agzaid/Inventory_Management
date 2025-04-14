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
        Task<List<SelectListItem>> ForCartView();
        Task<Result<string>> CreateOrder(CartVM cart);
        PortalVM GetAllProductsForPortal();
        ProductVM GetProductDetails(int id);
        ProductVM GetProductById(int id);
        Result<List<OnlineOrderVM>> GetAllOrdersToBeInvoiced();
        Result<InvoiceVM> GetInvoiceForSpecificOnlineOrder(int id);
    }
}
