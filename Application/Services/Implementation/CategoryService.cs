using Application.Common.Interfaces;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryService> _logger;  // Inject ILogger<CategoryService>

        public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;  // Initialize the logger
        }

        public IEnumerable<CategoryVM> GetAllCategories()
        {
            try
            {
                var categories = _unitOfWork.Category.GetAll(s => s.IsDeleted == false);
                var showCategories = categories.Select(s => new CategoryVM()
                {
                    Id = s.Id,
                    Description = s.Description,
                    CategoryName = s.CategoryName,
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                }).ToList();

                _logger.LogInformation("GetAllCategories method completed. {CategoryCount} categories retrieved.", showCategories.Count);

                return showCategories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving categories.");
                throw;  // Rethrow the exception after logging it
            }
        }

        public async Task<string> CreateCategory(CategoryVM obj)
        {
            try
            {
                obj.CategoryName = obj.CategoryName?.ToLower();
                obj.Description = obj.Description?.ToLower();

                var lookForName = await _unitOfWork.Category.GetFirstOrDefaultAsync(
                    s => s.CategoryName == obj.CategoryName
                );

                if (lookForName == null)
                {
                    var category = new Category
                    {
                        CategoryName = obj.CategoryName,
                        CategoryNameAr = obj.CategoryNameAr,
                        Modified_Date = DateTime.UtcNow,
                        Description = obj.Description
                    };

                    await _unitOfWork.Category.AddAsync(category);
                    await _unitOfWork.SaveAsync();
                    return "Category Created Successfully";
                }
                else
                {
                    return "Category Already Exists";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "An error occurred while creating category with CategoryName: {CategoryName}",
                    obj.CategoryName
                );
                return "Error Occurred...";
            }
        }


        public async Task<CategoryVM> GetCategoryById(int id)
        {
            try
            {
                var category = await _unitOfWork.Category.GetAsync(u => u.Id == id);
                if (category != null)
                {
                    var categoryVM = new CategoryVM()
                    {
                        CategoryName = category.CategoryName,
                        CategoryNameAr = category.CategoryNameAr,
                        Description = category.Description,
                        CreatedDate = category.Create_Date?.ToString("yyyy-MM-dd")
                    };
                    return categoryVM;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving category with Id: {Id}", id);
                throw;  // Rethrow the exception after logging it
            }
            return new CategoryVM();
        }

        public async Task<bool> UpdateCategory(CategoryVM obj)
        {
            try
            {
                var oldCategory = await _unitOfWork.Category
                    .GetFirstOrDefaultAsync(s => s.Id == obj.Id, null, true);

                if (oldCategory == null)
                    return false;

                // Normalize data
                obj.CategoryName = obj.CategoryName?.ToLower();
                obj.Description = obj.Description?.ToLower();

                // Update fields
                oldCategory.CategoryNameAr = obj.CategoryNameAr;
                oldCategory.Description = obj.Description;
                oldCategory.CategoryName = obj.CategoryName;
                oldCategory.Modified_Date = DateTime.UtcNow;

                _unitOfWork.Category.Update(oldCategory);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "An error occurred while updating category with Id: {Id}", obj.Id);
                return false;
            }
        }


        public async Task<bool> DeleteCategory(int id)
        {
            try
            {
                var oldCategory = await _unitOfWork.Category
                    .GetFirstOrDefaultAsync(s => s.Id == id, null, true);

                if (oldCategory == null)
                    return false;

                oldCategory.IsDeleted = true;
                oldCategory.Modified_Date = DateTime.UtcNow;

                _unitOfWork.Category.Update(oldCategory);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "An error occurred while deleting category with Id: {Id}", id);
                return false;
            }
        }

        public async Task<PaginatedResult<CategoryVM>> GetCategoryPaginated(int pageNumber, int pageSize)
        {
            try
            {
                Expression<Func<Category, bool>> filter = s => s.IsDeleted == false;
                Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy;
                orderBy = s => s.OrderByDescending(s => s.CategoryName);

                var categories = await _unitOfWork.Category.GetPaginatedAsync(pageNumber, pageSize, orderBy, filter);
                var showCategories = categories.Items.Select(s => new CategoryVM()
                {
                    Id = s.Id,
                    Description = s.Description,
                    CategoryNameAr = s.CategoryNameAr,
                    CategoryName = s.CategoryName,
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                }).ToList();
                var paginatedResult = new PaginatedResult<CategoryVM>
                {
                    Items = showCategories,
                    TotalCount = categories.TotalCount,
                    PageNumber = categories.PageNumber,
                    PageSize = categories.PageSize
                };
                return paginatedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Getting category paginated");
                throw;
            }

        }
    }
}
