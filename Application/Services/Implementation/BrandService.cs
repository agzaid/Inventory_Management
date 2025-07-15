using Application.Common.Interfaces;
using Application.Common.Utility;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Http;
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
            var imagesToBeDeleted = new List<string>();
            var imagesToBeRemoved = new List<byte[]>();
            try
            {
                //obj.BrandName = obj.BrandName?.ToLower();
                obj.Description = obj.Description?.ToLower();
                var lookForName = _unitOfWork.Brand.Get(s => s.BrandName == obj.BrandName);
                if (obj?.ImagesFormFiles?.Count() > 0)
                {
                    var resultByteImage = new byte[0];
                    //result = await FileExtensions.CreateImages(product.ImagesFormFiles, product?.ProductName);
                    foreach (var item in obj.ImagesFormFiles)
                    {
                        resultByteImage = FileExtensions.ConvertImageToByteArray(item);
                        imagesToBeRemoved.Add(resultByteImage);
                    }
                }
                //imagesToBeDeleted = result;
                var listOfImages = imagesToBeRemoved.Select(s => new Domain.Entities.Image()
                {
                    ImageByteArray = s ?? new byte[0],
                    Create_Date = DateTime.Now,
                }).ToList();
                if (lookForName == null)
                {
                    var Brand = new Brand()
                    {
                        BrandName = obj.BrandName,
                        BrandNameAr = obj.BrandNameAr,
                        Modified_Date = DateTime.Now,
                        Description = obj.Description,
                        Images = listOfImages,
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
                var Brand = _unitOfWork.Brand.Get(u => u.Id == id, "Images");
                if (Brand != null)
                {
                    var BrandVM = new BrandVM()
                    {
                        BrandName = Brand.BrandName,
                        BrandNameAr = Brand.BrandNameAr,
                        Description = Brand.Description,
                        CreatedDate = Brand.Create_Date?.ToString("yyyy-MM-dd")
                    };
                    if (Brand.Images?.Count() > 0)
                    {
                        foreach (var item in Brand.Images)
                        {
                            //var s = FileExtensions.ByteArrayToImage(item.ImageByteArray);
                            if (item.ImageByteArray?.Length > 0)
                            {
                                var stringImages = FileExtensions.ByteArrayToImageBase64(item.ImageByteArray);
                                BrandVM.ListOfRetrievedImages?.Add(stringImages);
                            }
                        }
                    }
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
                var imagesToBeInserted = new List<byte[]>();

                // Load old brand with its images
                var oldBrand = _unitOfWork.Brand.Get(s => s.Id == obj.Id, "Images");

                // Remove old images
                RemoveOldImages(oldBrand);

                // Prepare new images
                AddNewImages(obj.ImagesFormFiles, obj.OldImagesBytes, imagesToBeInserted);

                // Create image entities
                var listOfImages = CreateImageEntities(imagesToBeInserted);

                // Update brand properties
                if (oldBrand != null)
                {
                    oldBrand.BrandName = obj.BrandName?.ToLower();
                    oldBrand.BrandNameAr = obj.BrandNameAr;
                    oldBrand.Description = obj.Description?.ToLower();
                    oldBrand.Modified_Date = DateTime.UtcNow;
                    oldBrand.Images = listOfImages;

                    _unitOfWork.Brand.Update(oldBrand);
                    _unitOfWork.Save();
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
