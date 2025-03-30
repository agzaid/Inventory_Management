using Domain.Entities;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Intrerfaces
{
    public interface IDistrictService
    {
        DistrictVM GetDistrictForCreateView();
        Result<IEnumerable<DistrictVM>> GetAllDistrics();
        DistrictVM GetDistricById(int id);
        Task<Result<string>> CreateDistrict(DistrictVM shippingFrieghtVM);
        Task<bool> UpdateDistrict(DistrictVM ShippingFreightvm);
        Task<bool> DeleteDistrict(int id);
    }
}
