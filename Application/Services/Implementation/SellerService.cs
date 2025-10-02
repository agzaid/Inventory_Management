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
    public class SellerService : ISellerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SellerService> _logger;  // Inject ILogger<SellerService>

        public SellerService(IUnitOfWork unitOfWork, ILogger<SellerService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;  // Initialize the logger
        }

        public IEnumerable<SellerVM> GetAllSellers()
        {
            try
            {
                var Sellers = _unitOfWork.Seller.GetAll(s => s.IsDeleted == false);
                var showSellers = Sellers.Select(s => new SellerVM()
                {
                    Id = s.Id,
                    Description = s.Description,
                    SellerName = s.SellerName,
                    SellerNameAr = s.SellerNameAr,
                    Address = s.Address,
                    PhoneNumber = s.PhoneNumber,
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                }).ToList();

                _logger.LogInformation("GetAllSellers method completed. {SellerCount} Sellers retrieved.", showSellers.Count);

                return showSellers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving Sellers.");
                throw;  // Rethrow the exception after logging it
            }
        }

        public async Task<Result<string>> CreateSeller(SellerVM obj)
        {
            try
            {
                // normalize inputs
                obj.SellerName = obj.SellerName?.ToLower();
                obj.Description = obj.Description?.ToLower();

                // check if Seller exists
                var existingSeller = await _unitOfWork.Seller
                    .GetFirstOrDefaultAsync(s => s.SellerName.ToLower() == obj.SellerName);

                if (existingSeller != null)
                {
                    return Result<string>.Failure("Seller already exists", "duplicate");
                }

                var imagesToBeAdded = new List<string>();

                // save images if provided
                if (obj?.ImagesFormFiles?.Count > 0)
                {
                    foreach (var item in obj.ImagesFormFiles)
                    {
                        var resultImagePath = await FileExtensions.SaveImageOptimized(item, "Seller");
                        imagesToBeAdded.Add(resultImagePath);
                    }
                }

                // map to Image entities
                var listOfImages = imagesToBeAdded.Select(s => new Domain.Entities.Image
                {
                    FilePath = s,
                    Create_Date = DateTime.Now
                }).ToList();

                // create Seller entity
                var Seller = new Seller
                {
                    SellerName = obj.SellerName,
                    SellerNameAr = obj.SellerNameAr,
                    Description = obj.Description,
                    Modified_Date = DateTime.Now,
                    Images = listOfImages
                };

                await _unitOfWork.Seller.AddAsync(Seller);
                await _unitOfWork.SaveAsync();

                return Result<string>.Success("success", "Seller Created Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating Seller with SellerName: {SellerName}", obj.SellerName);
                return Result<string>.Failure("Error Occurred while creating Seller", "error");
            }
        }


        public SellerVM GetSellerById(int id)
        {
            try
            {
                var Seller = _unitOfWork.Seller.Get(u => u.Id == id, "Images");
                if (Seller != null)
                {
                    var SellerVM = new SellerVM()
                    {
                        SellerName = Seller.SellerName,
                        SellerNameAr = Seller.SellerNameAr,
                        PhoneNumber = Seller.PhoneNumber,
                        Address = Seller.Address,
                        Description = Seller.Description,
                        CreatedDate = Seller.Create_Date?.ToString("yyyy-MM-dd"),
                        ListOfRetrievedImages = Seller.Images?.Select(s => s.FilePath).ToList()
                    };

                    return SellerVM;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving Seller with Id: {Id}", id);
                throw;  // Rethrow the exception after logging it
            }
            return new SellerVM();
        }

        public async Task<bool> UpdateSeller(SellerVM obj)
        {
            try
            {
                // Load old Seller with its images (tracked)
                var oldSeller = await _unitOfWork.Seller.GetFirstOrDefaultAsync(
                    s => s.Id == obj.Id,
                    "Images",
                    true);

                if (oldSeller != null)
                {
                    // Update main Seller properties
                    oldSeller.SellerName = obj.SellerName?.ToLower();
                    oldSeller.SellerNameAr = obj.SellerNameAr;
                    oldSeller.PhoneNumber = obj.PhoneNumber;
                    oldSeller.Address = obj.Address;
                    oldSeller.Description = obj.Description?.ToLower();
                    oldSeller.Modified_Date = DateTime.UtcNow;

                    // 1. Remove unwanted old images
                    var imagesToBeRemoved = oldSeller.Images
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
                        var resultImagePaths = await FileExtensions.SaveImagesOptimized(obj.ImagesFormFiles, "Seller");

                        var newImages = resultImagePaths.Select(s => new Domain.Entities.Image
                        {
                            FilePath = s,
                            Create_Date = DateTime.Now,
                        }).ToList();

                        foreach (var img in newImages)
                        {
                            oldSeller.Images.Add(img);
                        }
                    }

                    // 3. Save changes
                    _unitOfWork.Seller.Update(oldSeller);
                    await _unitOfWork.SaveAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating Seller with Id: {Id}", obj.Id);
                return false;
            }
        }

        public async Task<bool> DeleteSeller(int id)
        {
            try
            {
                var oldSeller = _unitOfWork.Seller.Get(s => s.Id == id);
                if (oldSeller != null)
                {
                    oldSeller.IsDeleted = true;
                    oldSeller.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.Seller.Update(oldSeller);
                    await _unitOfWork.SaveAsync();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting Seller with Id: {Id}", id);
                return false; // Rethrow the exception after logging it
            }
        }
        public async Task<PaginatedResult<SellerVM>> GetSellerPaginated(int pageNumber, int pageSize)
        {
            try
            {
                Expression<Func<Seller, bool>> filter = s => s.IsDeleted == false;
                Func<IQueryable<Seller>, IOrderedQueryable<Seller>> orderBy;
                orderBy = s => s.OrderByDescending(s => s.SellerName);

                var Sellers = await _unitOfWork.Seller.GetPaginatedAsync(pageNumber, pageSize, orderBy, filter,x=>x.Images);
                var showSellers = Sellers.Items.Select(s => new SellerVM()
                {
                    Id = s.Id,
                    Description = s.Description,
                    SellerNameAr = s.SellerNameAr,
                    Address=s.Address,
                    PhoneNumber = s.PhoneNumber,
                    
                    SellerName = s.SellerName,
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                    ListOfRetrievedImages = s.Images?.Select(img => img.FilePath).ToList()
                }).ToList();
                var paginatedResult = new PaginatedResult<SellerVM>
                {
                    Items = showSellers,
                    TotalCount = Sellers.TotalCount,
                    PageNumber = Sellers.PageNumber,
                    PageSize = Sellers.PageSize
                };
                return paginatedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Getting Seller paginated");
                throw;
            }

        }
        // Method to remove old images from the product
        private void RemoveOldImages(Seller oldProduct)
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
