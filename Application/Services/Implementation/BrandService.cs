using Application.Common.Interfaces;
using Application.Common.Utility;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Drawing.Drawing2D;
using System.Linq.Expressions;
using System.Web.Mvc;

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

        //public async Task<string> CreateBrand(BrandVM obj)
        //{
        //    var imagesToBeDeleted = new List<string>();
        //    var imagesToBeRemoved = new List<string>();
        //    try
        //    {
        //        obj.BrandName = obj.BrandName?.ToLower();
        //        obj.Description = obj.Description?.ToLower();
        //        var lookForName = await _unitOfWork.Brand.GetFirstOrDefaultAsync(s => s.BrandName.ToLower() == obj.BrandName);
        //        if (obj?.ImagesFormFiles?.Count() > 0)
        //        {
        //            var resultByteImage = new byte[0];
        //            //result = await FileExtensions.CreateImages(product.ImagesFormFiles, product?.ProductName);
        //            foreach (var item in obj.ImagesFormFiles)
        //            {
        //                var resultImagePath = await FileExtensions.SaveImageOptimized(item, "Brand");

        //                imagesToBeRemoved.Add(resultImagePath);
        //            }
        //        }
        //        //imagesToBeDeleted = result;
        //        var listOfImages = imagesToBeRemoved.Select(s => new Domain.Entities.Image()
        //        {
        //            FilePath = s,
        //            Create_Date = DateTime.Now,
        //        }).ToList();
        //        if (lookForName == null)
        //        {
        //            var Brand = new Brand()
        //            {
        //                BrandName = obj.BrandName,
        //                BrandNameAr = obj.BrandNameAr,
        //                Modified_Date = DateTime.Now,
        //                Description = obj.Description,
        //                Images = listOfImages,
        //            };
        //            await _unitOfWork.Brand.AddAsync(Brand);
        //            await _unitOfWork.SaveAsync();
        //            return "Brand Created Successfully";
        //        }
        //        else
        //            return "Brand Already Exists";
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while creating Brand with BrandName: {BrandName}", obj.BrandName);
        //        return "Error Occured...";  // Rethrow the exception after logging it
        //    }
        //}
        public async Task<Result<string>> CreateBrand(BrandVM obj)
        {
            try
            {
                // normalize inputs
                obj.BrandName = obj.BrandName?.ToLower();
                obj.Description = obj.Description?.ToLower();

                // check if brand exists
                var existingBrand = await _unitOfWork.Brand
                    .GetFirstOrDefaultAsync(s => s.BrandName.ToLower() == obj.BrandName,"BrandsCategories");

                if (existingBrand != null)
                {
                    return Result<string>.Failure("Brand already exists", "duplicate");
                }

                var imagesToBeAdded = new List<string>();

                // save images if provided
                if (obj?.ImagesFormFiles?.Count > 0)
                {
                    foreach (var item in obj.ImagesFormFiles)
                    {
                        var resultImagePath = await FileExtensions.SaveImageOptimized(item, "Brand");
                        imagesToBeAdded.Add(resultImagePath);
                    }
                }

                // map to Image entities
                var listOfImages = imagesToBeAdded.Select(s => new Domain.Entities.Image
                {
                    FilePath = s,
                    Create_Date = DateTime.Now
                }).ToList();


                // create brand entity
                var brand = new Brand
                {
                    BrandName = obj.BrandName,
                    BrandNameAr = obj.BrandNameAr,
                    Description = obj.Description,
                    Modified_Date = DateTime.Now,
                    Images = listOfImages
                };
                if (obj.CategoryIds != null && obj.CategoryIds.Any())
                {
                    // get all existing brand-category relations (if any)
                    var existingRelations = await _unitOfWork.BrandsCategories
                        .GetAllAsync(x => obj.CategoryIds.Contains(x.CategoryId) && x.BrandId == brand.Id);

                    var existingCategoryIds = existingRelations.Select(x => x.CategoryId).ToHashSet();

                    var newRelations = obj.CategoryIds
                        .Where(catId => !existingCategoryIds.Contains(catId))
                        .Select(catId => new BrandsCategories
                        {
                            BrandId = brand.Id,
                            CategoryId = catId,
                            Create_Date = DateTime.Now
                        })
                        .ToList();

                    brand.BrandsCategories = newRelations;
                }

                await _unitOfWork.Brand.AddAsync(brand);
                await _unitOfWork.SaveAsync();

                return Result<string>.Success("success", "Brand Created Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating Brand with BrandName: {BrandName}", obj.BrandName);
                return Result<string>.Failure("Error Occurred while creating brand", "error");
            }
        }

        public async Task<BrandVM> GetBrandForCreateViewAsync()
        {
            try
            {
                var categories = await _unitOfWork.Category.GetAllAsync(s => s.IsDeleted == false);
                var categoriesList = categories.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.CategoryName
                }).ToList();
                var brand = new BrandVM()
                {
                    CategoryList = categories.Select(s => new SelectListItem
                    {
                        Text = s.CategoryName,
                        Value = s.Id.ToString()
                    }).ToList(),
                };

                //_logger.LogInformation("GetAllShippingFrieght method completed. {allShippingFrieghtCount} allShippingFrieght retrieved.", allShippingFrieght.ToList().Count);

                return brand;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving categories.");
                throw;  // Rethrow the exception after logging it
            }
        }
        public BrandVM GetBrandById(int id)
        {
            try
            {
                var categories = _unitOfWork.Category.GetAll(s => s.IsDeleted == false);
                var categoryList = categories.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.CategoryName
                }).ToList();
                var Brand = _unitOfWork.Brand.Get(u => u.Id == id, "Images, BrandsCategories");
                if (Brand != null)
                {
                    var BrandVM = new BrandVM()
                    {
                        BrandName = Brand.BrandName,
                        BrandNameAr = Brand.BrandNameAr,
                        Description = Brand.Description,
                        //CategoryId = Brand.CategoryId,
                        CreatedDate = Brand.Create_Date?.ToString("yyyy-MM-dd"),
                        ListOfRetrievedImages = Brand.Images?.Select(s => s.FilePath).ToList(),
                        CategoryList = categoryList,
                        // Fix for CS0029 and CS8601
                        CategoryIds = Brand.BrandsCategories?
                            .Select(bc => bc.CategoryId.HasValue ? bc.CategoryId.Value : (int?)null)
                            .ToList() ?? new List<int?>(),
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
        public async Task<BrandVM> GetBrandsByCategory(int? id)
        {
            try
            {
                var Brand = await _unitOfWork.Brand.GetAsync(u => true, "Images");
                if (Brand != null)
                {
                    var BrandVM = new BrandVM()
                    {
                        BrandName = Brand.BrandName,
                        BrandNameAr = Brand.BrandNameAr,
                        Description = Brand.Description,
                        CreatedDate = Brand.Create_Date?.ToString("yyyy-MM-dd"),
                        ListOfRetrievedImages = Brand.Images?.Select(s => s.FilePath).ToList()
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
                // Load old brand with its images (tracked)
                var oldBrand = await _unitOfWork.Brand.GetFirstOrDefaultAsync(
                    s => s.Id == obj.Id,
                    "Images,BrandsCategories",
                    true);

                if (oldBrand != null)
                {
                    // Update main Brand properties
                    oldBrand.BrandName = obj.BrandName?.ToLower();
                    oldBrand.BrandNameAr = obj.BrandNameAr;
                    oldBrand.Description = obj.Description?.ToLower();
                    oldBrand.Modified_Date = DateTime.UtcNow;
                    //oldBrand.CategoryId = obj.CategoryId;

                    // 1. Remove unwanted old images
                    var imagesToBeRemoved = oldBrand.Images
                        .Where(s => !obj.OldImagesBytes.Contains(s.FilePath))
                        .ToList();

                    foreach (var img in imagesToBeRemoved)
                    {
                        _unitOfWork.Image.Remove(img); // remove from DB
                        await FileExtensions.DeleteImages(new List<string> { img.FilePath }); // remove from wwwroot
                    }

                    // 2. Add new uploaded images
                    if (obj.ImagesFormFiles?.Count > 0)
                    {
                        var resultImagePaths = await FileExtensions.SaveImagesOptimized(obj.ImagesFormFiles, "Brand");

                        var newImages = resultImagePaths.Select(s => new Domain.Entities.Image
                        {
                            FilePath = s,
                            Create_Date = DateTime.Now,
                        }).ToList();

                        foreach (var img in newImages)
                        {
                            oldBrand.Images.Add(img);
                        }
                    }
                    // --- 4. Update brand-category relations ---
                    var newCategoryIds = obj.CategoryIds;
                    var existingBrandCategories = oldBrand.BrandsCategories.ToList();

                    // Categories to remove
                    var categoriesToRemove = existingBrandCategories
                        .Where(bc => !newCategoryIds.Contains(bc.CategoryId))
                        .ToList();

                    //if (categoriesToRemove.Any())
                    //    _unitOfWork.BrandsCategories.RemoveRange(categoriesToRemove);
                    foreach (var item in categoriesToRemove)
                    {
                        await _unitOfWork.BrandsCategories.RemoveAsync(item);
                    }
                    // Categories to add
                    var existingCategoryIds = existingBrandCategories.Select(bc => bc.CategoryId).ToHashSet();
                    var categoriesToAdd = newCategoryIds
                        .Where(catId => !existingCategoryIds.Contains(catId))
                        .Select(catId => new BrandsCategories
                        {
                            BrandId = oldBrand.Id,
                            CategoryId = catId,
                            Create_Date = DateTime.UtcNow
                        }).ToList();

                    foreach (var bc in categoriesToAdd)
                        oldBrand.BrandsCategories.Add(bc);

                    // --- 5. Save all changes ---
                    _unitOfWork.Brand.Update(oldBrand);
                    await _unitOfWork.SaveAsync();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating Brand with Id: {Id}", obj.Id);
                return false;
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
                    await _unitOfWork.SaveAsync();
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

                var Brands = await _unitOfWork.Brand.GetPaginatedAsync(pageNumber, pageSize, orderBy, filter, x => x.Images);
                var showBrands = Brands.Items.Select(s => new BrandVM()
                {
                    Id = s.Id,
                    Description = s.Description,
                    BrandNameAr = s.BrandNameAr,
                    BrandName = s.BrandName,
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                    ListOfRetrievedImages = s.Images?.Select(img => img.FilePath).ToList()
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
        // Method to remove old images from the product
        private void RemoveOldImages(Brand oldProduct)
        {
            if (oldProduct?.Images?.Count > 0)
            {
                _logger.LogInformation("Removing old images for product with Id: {Id}", oldProduct.Id);
                foreach (var item in oldProduct.Images)
                {
                    _unitOfWork.Image.Remove(item);
                }
                oldProduct.Images.Clear();
                _logger.LogInformation("Old images removed.");
            }
        }
        // Method to add new images (from form files and old image bytes)
        private void AddNewImages(IList<IFormFile> newImages, IList<string> oldImages, List<byte[]> imagesToBeInserted)
        {
            if (newImages?.Count > 0)
            {
                foreach (var item in newImages)
                {
                    var byteImage = FileExtensions.ConvertImageToByteArray(item);
                    imagesToBeInserted.Add(byteImage);
                }
            }

            if (oldImages?.Count > 0)
            {
                foreach (var item in oldImages)
                {
                    var byteImage = FileExtensions.FromImageToByteArray(item);
                    imagesToBeInserted.Add(byteImage);
                }
            }
        }

        // Method to create Image entities from byte arrays
        private List<Domain.Entities.Image> CreateImageEntities(List<byte[]> imagesToBeInserted)
        {
            return imagesToBeInserted.Select(image => new Domain.Entities.Image()
            {
                ImageByteArray = image ?? new byte[0],
                Create_Date = DateTime.Now,
            }).ToList();
        }

    }
}
