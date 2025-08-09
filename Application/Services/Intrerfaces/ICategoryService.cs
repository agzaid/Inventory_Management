using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Intrerfaces
{
    public interface ICategoryService
    {
        IEnumerable<CategoryVM> GetAllCategories();
        Task<string> CreateCategory(CategoryVM category);
        Task<bool> UpdateCategory(CategoryVM category);
        Task<bool> DeleteCategory(int id);
        Task<PaginatedResult<CategoryVM>> GetCategoryPaginated(int pageNumber, int pageSide);
        Task<CategoryVM> GetCategoryById(int id);
    }
}
