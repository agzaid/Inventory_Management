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
        IEnumerable<InvoiceVM> GetAllInvoices();
        InvoiceVM GetInvoiceById(int id);
        Task<string[]> CreateInvoice(InvoiceVM invoice);
        Result<List<ProductVM>> SearchForProducts(string search);
        Task<Result<CustomerVM>> SearchForCustomer(string search);
        InvoiceVM CreateInvoiceForViewing();
        Task<InvoiceVM> GetInvoiceById();
        Task<bool> UpdateInvoice(ProductVM productVM);
        Task<bool> DeleteInvoice(int id);
        Task<bool> HardDeleteInvoice(int id);
        Task<List<MonthlyInventoryVM>> GetReportByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
