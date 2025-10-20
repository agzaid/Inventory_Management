using Application.Common.Interfaces;
using Application.Common.Utility;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;

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
                    Slug = s.Slug,
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
        public async Task<CategoryVM> CreateViewForCategory()
        {
            var brands = await _unitOfWork.Brand.GetAllAsync(s => s.IsDeleted == false);
            var brandsList = brands.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.BrandName
            }).ToList();

            var model = new CategoryVM
            {
                AvailableBrands = brandsList
            };
            return model;
        }

        public async Task<string> CreateCategory(CategoryVM obj)
        {
            try
            {
                obj.CategoryName = obj.CategoryName?.ToLower();
                obj.Description = obj.Description?.ToLower();

                var lookForName = await _unitOfWork.Category.GetFirstOrDefaultAsync(
                    s => s.CategoryName == obj.CategoryName, "BrandsCategories"
                );

                if (lookForName == null)
                {
                    var category = new Category
                    {
                        CategoryName = obj.CategoryName,
                        CategoryNameAr = obj.CategoryNameAr,
                        Slug = SlugGenerator.GenerateSlug(obj.CategoryName ?? string.Empty),
                        Modified_Date = DateTime.UtcNow,
                        Description = obj.Description
                    };
                    if (obj.SelectedBrandIds != null && obj.SelectedBrandIds.Any())
                    {
                        category.BrandsCategories = obj.SelectedBrandIds.Select(brandId => new BrandsCategories
                        {
                            BrandId = brandId
                        }).ToList();
                    }
                    if (obj.SelectedBrandIds != null && obj.SelectedBrandIds.Any())
                    {
                        // Get any existing relations (though the category is new, this is defensive)
                        var existingRelations = await _unitOfWork.BrandsCategories
                            .GetAllAsync(x => obj.SelectedBrandIds.Contains(x.BrandId ?? 0) && x.CategoryId == category.Id);

                        var existingBrandIds = existingRelations.Select(x => x.BrandId).ToHashSet();

                        var newRelations = obj.SelectedBrandIds
                            .Where(brandId => !existingBrandIds.Contains(brandId))
                            .Select(brandId => new BrandsCategories
                            {
                                BrandId = brandId,
                                Create_Date = DateTime.UtcNow
                            })
                            .ToList();

                        category.BrandsCategories = newRelations;
                    }

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
                var brands = await _unitOfWork.Brand.GetAllAsync(s => s.IsDeleted == false);
                var brandsList = brands.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.BrandName
                }).ToList();
                var category = await _unitOfWork.Category.GetAsync(u => u.Id == id, "BrandsCategories");
                if (category != null)
                {
                    var categoryVM = new CategoryVM()
                    {
                        CategoryName = category.CategoryName,
                        CategoryNameAr = category.CategoryNameAr,
                        Slug = category.Slug,
                        Description = category.Description,
                        CreatedDate = category.Create_Date?.ToString("yyyy-MM-dd"),
                        AvailableBrands = brandsList,
                        SelectedBrandIds = category.BrandsCategories?
                            .Select(bc => bc.BrandId ?? 0) // Use null-coalescing operator to handle nullable int
                            .ToList()
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
                    .GetFirstOrDefaultAsync(s => s.Id == obj.Id, includeProperties: "BrandsCategories", tracked: true);

                if (oldCategory == null)
                    return false;

                // Normalize data
                obj.CategoryName = obj.CategoryName?.ToLower();
                obj.Description = obj.Description?.ToLower();

                // Existing BrandIds in DB
                var existingBrandIds = oldCategory.BrandsCategories
                    .Select(bc => bc.BrandId)
                    .ToList();

                // New BrandIds from the form
                var newBrandIds = obj.SelectedBrandIds ?? new List<int>();

                // Find brands to remove (exist in DB but not in new list)
                var brandsToRemove = oldCategory.BrandsCategories
                    .Where(bc => !newBrandIds.Contains(bc.BrandId ?? 0))
                    .ToList();

                foreach (var item in brandsToRemove)
                    oldCategory.BrandsCategories.Remove(item);

                // Find brands to add (exist in new list but not in DB)
                var brandsToAdd = newBrandIds
                    .Where(id => !existingBrandIds.Contains(id))
                    .Select(id => new BrandsCategories { BrandId = id })
                    .ToList();

                foreach (var item in brandsToAdd)
                    oldCategory.BrandsCategories.Add(item);

                // Update fields
                oldCategory.CategoryNameAr = obj.CategoryNameAr;
                oldCategory.Slug = SlugGenerator.GenerateSlug(obj.CategoryName ?? string.Empty);
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
                    Slug = s.Slug,
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
