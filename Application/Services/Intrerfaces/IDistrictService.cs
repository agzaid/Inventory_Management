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
        Task<Result<string>> CreateDistrict(DistrictVM shippingFrieghtVM);
        Task<bool> UpdateDistrict(DistrictVM ShippingFreightvm);
        Task<bool> DeleteDistrict(int id);
        Task<DistrictVM> GetDistricById(int id);
        Result<IEnumerable<DistrictVM>> GetAllDistricts();
    }
}
