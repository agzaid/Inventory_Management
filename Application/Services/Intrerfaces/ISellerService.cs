using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Intrerfaces
{
    public interface ISellerService
    {
        IEnumerable<SellerVM> GetAllSellers();
        SellerVM GetSellerById(int id);
        //Task<string> CreateSeller(SellerVM category);
        Task<bool> UpdateSeller(SellerVM category);
        Task<bool> DeleteSeller(int id);
        Task<PaginatedResult<SellerVM>> GetSellerPaginated(int pageNumber, int pageSide);
        Task<Result<string>> CreateSeller(SellerVM obj);
    }
}
