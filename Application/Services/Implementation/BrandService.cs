using Application.Common.Interfaces;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Application.Services.Implementation
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BrandService> _logger;  // Inject ILogger<BrandService>

        public BrandService(IUnitOfWork unitOfWork, ILogger<BrandService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;  // Initialize the logger
        }

        public IEnumerable<BrandVM> GetAllBrands()
        {
            try
            {
                var brands = _unitOfWork.Brand.GetAll(s => s.IsDeleted == false);
                var showBrands = brands.Select(s => new BrandVM()
                {
                    Id = s.Id,
                    Description = s.Description,
                    BrandName = s.BrandName,
                    BrandNameAr = s.BrandNameAr,
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                }).ToList();

                _logger.LogInformation("GetAllBrands method completed. {BrandCount} Brands retrieved.", showBrands.Count);

                return showBrands;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving Brands.");
                throw;  // Rethrow the exception after logging it
            }
        }

        public async Task<string> CreateBrand(BrandVM obj)
        {
            try
            {
                //obj.BrandName = obj.BrandName?.ToLower();
                obj.Description = obj.Description?.ToLower();
                var lookForName = _unitOfWork.Brand.Get(s => s.BrandName == obj.BrandName);
                if (lookForName == null)
                {
                    var Brand = new Brand()
                    {
                        BrandName = obj.BrandName,
                        BrandNameAr = obj.BrandNameAr,
                        Modified_Date = DateTime.Now,
                        Description = obj.Description,
                    };
                    _unitOfWork.Brand.Add(Brand);
                    _unitOfWork.Save();
                    return "Brand Created Successfully";
                }
                else
                    return "Brand Already Exists";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating Brand with BrandName: {BrandName}", obj.BrandName);
                return "Error Occured...";  // Rethrow the exception after logging it
            }
        }

        public BrandVM GetBrandById(int id)
        {
            try
            {
                var Brand = _unitOfWork.Brand.Get(u => u.Id == id);
                if (Brand != null)
                {
                    var BrandVM = new BrandVM()
                    {
                        BrandName = Brand.BrandName,
                        BrandNameAr = Brand.BrandNameAr,
                        Description = Brand.Description,
                        CreatedDate = Brand.Create_Date?.ToString("yyyy-MM-dd")
                    };
                    return BrandVM;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving Brand with Id: {Id}", id);
                throw;  // Rethrow the exception after logging it
            }
            return new BrandVM();
        }

        public async Task<bool> UpdateBrand(BrandVM obj)
        {
            try
            {
                obj.BrandName = obj.BrandName?.ToLower();
                obj.Description = obj.Description?.ToLower();
                var oldBrand = _unitOfWork.Brand.Get(s => s.Id == obj.Id);
                if (oldBrand != null)
                {
                    oldBrand.BrandNameAr = obj.BrandNameAr;
                    oldBrand.Description = obj.Description;
                    oldBrand.BrandName = obj.BrandName;
                    oldBrand.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.Brand.Update(oldBrand);
                    _unitOfWork.Save();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating Brand with Id: {Id}", obj.Id);
                return false;  // Rethrow the exception after logging it
            }
        }

        public async Task<bool> DeleteBrand(int id)
        {
            try
            {
                var oldBrand = _unitOfWork.Brand.Get(s => s.Id == id);
                if (oldBrand != null)
                {
                    oldBrand.IsDeleted = true;
                    oldBrand.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.Brand.Update(oldBrand);
                    await _unitOfWork.Save();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting Brand with Id: {Id}", id);
                return false; // Rethrow the exception after logging it
            }
        }
        public async Task<PaginatedResult<BrandVM>> GetBrandPaginated(int pageNumber, int pageSize)
        {
            try
            {
                Expression<Func<Brand, bool>> filter = s => s.IsDeleted == false;
                Func<IQueryable<Brand>, IOrderedQueryable<Brand>> orderBy;
                orderBy = s => s.OrderByDescending(s => s.BrandName);

                var Brands = await _unitOfWork.Brand.GetPaginatedAsync(pageNumber, pageSize, orderBy, filter);
                var showBrands = Brands.Items.Select(s => new BrandVM()
                {
                    Id = s.Id,
                    Description = s.Description,
                    BrandNameAr = s.BrandNameAr,
                    BrandName = s.BrandName,
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                }).ToList();
                var paginatedResult = new PaginatedResult<BrandVM>
                {
                    Items = showBrands,
                    TotalCount = Brands.TotalCount,
                    PageNumber = Brands.PageNumber,
                    PageSize = Brands.PageSize
                };
                return paginatedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Getting Brand paginated");
                throw;
            }

        }
    }
}
