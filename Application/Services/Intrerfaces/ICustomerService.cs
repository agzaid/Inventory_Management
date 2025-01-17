using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Intrerfaces
{
    public interface ICustomerService
    {
        Result<IEnumerable<CustomerVM>> GetAllCustomers();
        CustomerVM CreateCustomerForViewing();
        Result<CustomerVM> GetCustomerById(int id);
        Task<Result<string>> CreateCustomer(CustomerVM customer);
        Task<bool> UpdateCustomer(CustomerVM customerVM);
        Task<bool> DeleteCustomer(int id);
    }
}
