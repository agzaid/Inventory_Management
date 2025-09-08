using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Intrerfaces
{
    public interface IBrandService
    {
        IEnumerable<BrandVM> GetAllBrands();
        BrandVM GetBrandById(int id);
        //Task<string> CreateBrand(BrandVM category);
        Task<bool> UpdateBrand(BrandVM category);
        Task<bool> DeleteBrand(int id);
        Task<PaginatedResult<BrandVM>> GetBrandPaginated(int pageNumber, int pageSide);
        Task<Result<string>> CreateBrand(BrandVM obj);
    }
}
