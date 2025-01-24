using Domain.Entities;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Intrerfaces
{
    public interface IInvoiceService
    {
        IEnumerable<ProductVM> GetAllInvoices();
        ProductVM GetInvoiceById(int id);
        Task<string[]> CreateInvoice(InvoiceVM invoice);
        Result<List<ProductVM>> SearchForProducts(string search);
        Result<CustomerVM> SearchForCustomer(string search);
        InvoiceVM CreateInvoiceForViewing();
        bool UpdateInvoice(ProductVM productVM);
        bool DeleteInvoice(int id);
        bool HardDeleteInvoice(int id);
    }
}
