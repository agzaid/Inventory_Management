using Application.Common.Interfaces;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Application.Services.Implementation
{
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ImageService> _logger;  // Inject ILogger<CategoryService>

        public ImageService(IUnitOfWork unitOfWork, ILogger<ImageService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;  // Initialize the logger
        }

        public async Task<IEnumerable<ImageVM>> GetAllImages()
        {
            try
            {
                var categories = await _unitOfWork.Image.GetAllAsync(s => s.IsDeleted == false);
                var showCategories = categories.Select(s => new ImageVM()
                {
                    Id = s.Id,

                    //CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
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

        public async Task<string> CreateImage(ImageVM obj)
        {
            try
            {
                obj.ImageName = obj.ImageName?.ToLower();
                //obj.Description = obj.Description?.ToLower();
                var lookForName = await _unitOfWork.Image.GetFirstOrDefaultAsync(s => s.ImageName == obj.ImageName);
                if (lookForName == null)
                {
                    var image = new Image()
                    {
                        ImageName = obj.ImageName,
                        //Modified_Date = DateTime.Now,
                        //Description = obj.Description,
                    };
                    await _unitOfWork.Image.AddAsync(image);
                    await _unitOfWork.SaveAsync();
                    return "Category Created Successfully";
                }
                else
                    return "Category Already Exists";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating image with ImageName: {ImageName}", obj.ImageName);
                return "Error Occured...";  // Rethrow the exception after logging it
            }
        }

        public ImageVM GetImageById(int id)
        {
            try
            {
                var image = _unitOfWork.Image.Get(u => u.Id == id);
                if (image != null)
                {
                    var imageVM = new ImageVM()
                    {
                        ImageName = image.ImageName,
                        //Description = category.Description,
                        //CreatedDate = category.Create_Date?.ToString("yyyy-MM-dd")
                    };
                    return imageVM;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving category with Id: {Id}", id);
                throw;  // Rethrow the exception after logging it
            }
            return new ImageVM();
        }

        public async Task<bool> UpdateImage(ImageVM obj)
        {
            try
            {
                obj.ImageName = obj.ImageName?.ToLower();
                //obj.Description = obj.Description?.ToLower();
                var oldImage = await _unitOfWork.Image.GetFirstOrDefaultAsync(s => s.Id == obj.Id);
                if (oldImage != null)
                {
                    oldImage.ImageName = obj.ImageName;
                    //oldCategory.Description = obj.Description;
                    //oldCategory.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.Image.Update(oldImage);
                    await _unitOfWork.SaveAsync();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating category with Id: {Id}", obj.Id);
                return false;  // Rethrow the exception after logging it
            }
        }

        public async Task<bool> DeleteImage(int id)
        {
            try
            {
                var oldImage = await _unitOfWork.Image.GetFirstOrDefaultAsync(s => s.Id == id);
                if (oldImage != null)
                {
                    oldImage.IsDeleted = true;
                    oldImage.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.Image.Update(oldImage);
                    await _unitOfWork.SaveAsync();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting category with Id: {Id}", id);
                return false; // Rethrow the exception after logging it
            }
        }
    }
}
