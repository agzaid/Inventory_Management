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
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FeedbackService> _logger;  // Inject ILogger<Feedbackservice>

        public FeedbackService(IUnitOfWork unitOfWork, ILogger<FeedbackService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;  // Initialize the logger
        }

        public IEnumerable<FeedbackVM> GetAllFeedbacks()
        {
            try
            {
                var Feedbacks = _unitOfWork.Feedback.GetAll(s => s.IsDeleted == false);
                var showFeedbacks = Feedbacks.Select(s => new FeedbackVM()
                {
                    Id = s.Id,
                    Email = s.Email,
                    Name = s.Name,
                    Subject = s.Subject,
                    Message = s.Message,
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                }).ToList();

                _logger.LogInformation("GetAllFeedbacks method completed. {FeedbackCount} Feedbacks retrieved.", showFeedbacks.Count);

                return showFeedbacks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving Feedbacks.");
                throw;  // Rethrow the exception after logging it
            }
        }

        public async Task<string> CreateFeedback(FeedbackVM obj)
        {
            var imagesToBeDeleted = new List<string>();
            var imagesToBeRemoved = new List<byte[]>();
            try
            {
                //obj.FeedbackName = obj.FeedbackName?.ToLower();
                //obj.Description = obj.Description?.ToLower();
                var lookForName = await _unitOfWork.Feedback.GetFirstOrDefaultAsync(s => s.Email.ToLower() == obj.Email);
                //if (obj?.ImagesFormFiles?.Count() > 0)
                //{
                //    var resultByteImage = new byte[0];
                //    //result = await FileExtensions.CreateImages(product.ImagesFormFiles, product?.ProductName);
                //    foreach (var item in obj.ImagesFormFiles)
                //    {
                //        resultByteImage = FileExtensions.ConvertImageToByteArray(item);
                //        imagesToBeRemoved.Add(resultByteImage);
                //    }
                //}
                //imagesToBeDeleted = result;
                var listOfImages = imagesToBeRemoved.Select(s => new Domain.Entities.Image()
                {
                    ImageByteArray = s ?? new byte[0],
                    Create_Date = DateTime.Now,
                }).ToList();
                if (lookForName == null)
                {
                    var Feedback = new Feedback()
                    {
                        Email = obj.Email,
                        Name = obj.Name,
                        Modified_Date = DateTime.Now,
                        Message = obj.Message,
                        Subject = obj.Subject,
                        Create_Date = DateTime.Now,
                        //Images = listOfImages,
                    };
                    await _unitOfWork.Feedback.AddAsync(Feedback);
                    await _unitOfWork.SaveAsync();
                    return "Feedback Created Successfully";
                }
                else
                    return "Feedback Already Exists";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating Feedback with Name: {Name}", obj.Name);
                return "Error Occured...";  // Rethrow the exception after logging it
            }
        }

        public FeedbackVM GetFeedbackById(int id)
        {
            try
            {
                var Feedback = _unitOfWork.Feedback.Get(u => u.Id == id, "Images");
                if (Feedback != null)
                {
                    var FeedbackVM = new FeedbackVM()
                    {
                        Name = Feedback.Name,
                        Email = Feedback.Email,
                        Message = Feedback.Message,
                        Subject = Feedback.Subject,
                        CreatedDate = Feedback.Create_Date?.ToString("yyyy-MM-dd")
                    };
                    //if (Feedback.Images?.Count() > 0)
                    //{
                    //    foreach (var item in Feedback.Images)
                    //    {
                    //        //var s = FileExtensions.ByteArrayToImage(item.ImageByteArray);
                    //        if (item.ImageByteArray?.Length > 0)
                    //        {
                    //            var stringImages = FileExtensions.ByteArrayToImageBase64(item.ImageByteArray);
                    //            FeedbackVM.ListOfRetrievedImages?.Add(stringImages);
                    //        }
                    //    }
                    //}
                    return FeedbackVM;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving Feedback with Id: {Id}", id);
                throw;  // Rethrow the exception after logging it
            }
            return new FeedbackVM();
        }

        public async Task<bool> UpdateFeedback(FeedbackVM obj)
        {
            try
            {
                var imagesToBeInserted = new List<byte[]>();

                // Load old Feedback with its images
                var oldFeedback = await _unitOfWork.Feedback.GetFirstOrDefaultAsync(s => s.Id == obj.Id, "Images", true);

                //// Remove old images
                //RemoveOldImages(oldFeedback);

                //// Prepare new images
                //AddNewImages(obj.ImagesFormFiles, obj.OldImagesBytes, imagesToBeInserted);

                //// Create image entities
                //var listOfImages = CreateImageEntities(imagesToBeInserted);

                // Update Feedback properties
                if (oldFeedback != null)
                {
                    oldFeedback.Name = obj.Name?.ToLower();
                    oldFeedback.Email = obj.Email;
                    oldFeedback.Message = obj.Message;
                    oldFeedback.Subject = obj.Subject;
                    oldFeedback.Modified_Date = DateTime.UtcNow;
                    //oldFeedback.Images = listOfImages;

                    _unitOfWork.Feedback.Update(oldFeedback);
                    await _unitOfWork.SaveAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating Feedback with Id: {Id}", obj.Id);
                return false;
            }
        }

        public async Task<bool> DeleteFeedback(int id)
        {
            try
            {
                var oldFeedback = _unitOfWork.Feedback.Get(s => s.Id == id);
                if (oldFeedback != null)
                {
                    oldFeedback.IsDeleted = true;
                    oldFeedback.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.Feedback.Update(oldFeedback);
                    await _unitOfWork.SaveAsync();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting Feedback with Id: {Id}", id);
                return false; // Rethrow the exception after logging it
            }
        }
        public async Task<PaginatedResult<FeedbackVM>> GetFeedbackPaginated(int pageNumber, int pageSize)
        {
            try
            {
                Expression<Func<Feedback, bool>> filter = s => s.IsDeleted == false;
                Func<IQueryable<Feedback>, IOrderedQueryable<Feedback>> orderBy;
                orderBy = s => s.OrderByDescending(s => s.Email);

                var Feedbacks = await _unitOfWork.Feedback.GetPaginatedAsync(pageNumber, pageSize, orderBy, filter);
                var showFeedbacks = Feedbacks.Items.Select(s => new FeedbackVM()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Subject = s.Subject,
                    Message = s.Message,
                    Email = s.Email,
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                }).ToList();
                var paginatedResult = new PaginatedResult<FeedbackVM>
                {
                    Items = showFeedbacks,
                    TotalCount = Feedbacks.TotalCount,
                    PageNumber = Feedbacks.PageNumber,
                    PageSize = Feedbacks.PageSize
                };
                return paginatedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Getting Feedback paginated");
                throw;
            }

        }
        // Method to remove old images from the product
        //private void RemoveOldImages(Feedback oldProduct)
        //{
        //    if (oldProduct?.Images?.Count > 0)
        //    {
        //        _logger.LogInformation("Removing old images for product with Id: {Id}", oldProduct.Id);
        //        foreach (var item in oldProduct.Images)
        //        {
        //            _unitOfWork.Image.Remove(item);
        //        }
        //        oldProduct.Images.Clear();
        //        _logger.LogInformation("Old images removed.");
        //    }
        //}
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
