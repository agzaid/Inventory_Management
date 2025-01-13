using Domain.Entities;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Intrerfaces
{
    public interface IShippingFreightService
    {
        Result<IEnumerable<ShippingFreightVM>> GetAllShippingFrieght();
        ShippingFreightVM GetShippingFreightById(int id);
        Task<Result<string>> CreateShippingFreight(ShippingFreightVM shippingFrieghtVM);
        Task<bool> UpdateShippingFreight(ShippingFreightVM ShippingFreightvm);
        Task<bool> DeleteShippingFreight(int id);
    }
}
