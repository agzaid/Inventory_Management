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
                var feedbackVMs = new List<FeedbackVM>();

                var feedbacks = _unitOfWork.Feedback.GetAll(s => s.IsDeleted == false, "Images");

                foreach (var item in feedbacks)
                {
                    var image64 = item.Images?.Select(s => s.FilePath).ToList() ?? new List<string>();

                    var feedbackVM = new FeedbackVM
                    {
                        Id = item.Id,
                        Email = item.Email,
                        Name = item.Name,
                        Subject = item.Subject,
                        Message = item.Message,
                        Phone = item.Phone,
                        retrievedImages = image64,
                        CreatedDate = item.Create_Date?.ToString("yyyy-MM-dd")
                    };

                    feedbackVMs.Add(feedbackVM);
                }

                return feedbackVMs; // or wrap in your PortalVM
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving Feedbacks.");
                throw;  // Rethrow the exception after logging it
            }
        }

        public async Task<Result<string>> CreateFeedback(FeedbackVM feedback)
        {
            try
            {

                var customer = await _unitOfWork.Customer.GetFirstOrDefaultAsync(s => s.Phone == feedback.Phone);

                var imagesToBeDeleted = new List<string>();
                var imagesToBeAdded = new List<string>();

                if (feedback?.ImagesFormFiles?.Count() > 0)
                {
                    //result = await FileExtensions.CreateImages(product.ImagesFormFiles, product?.ProductName);
                    foreach (var item in feedback.ImagesFormFiles)
                    {
                        var resultImagePath = await FileExtensions.SaveImageOptimized(item, "Feedback");
                        imagesToBeAdded.Add(resultImagePath);
                    }
                }
                //imagesToBeDeleted = result;
                var listOfImages = imagesToBeAdded.Select(s => new Domain.Entities.Image()
                {
                    FilePath = s ,
                    Create_Date = DateTime.Now,
                }).ToList();

                var newFeedback = new Feedback()
                {
                    Name = feedback.Name,
                    Email = feedback.Email,
                    Subject = feedback.Subject,
                    Phone = feedback.Phone,
                    Message = feedback.Message,
                    CustomerId = 1,
                    Images = listOfImages
                };

                await _unitOfWork.Feedback.AddAsync(newFeedback);
                await _unitOfWork.SaveAsync();
                return Result<string>.Success("success", "Feedback Created Successfully");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex, ex.InnerException.Message);
                }
                else
                    _logger.LogError(ex, ex.Message);
                return Result<string>.Failure("Error Occured...", "error");
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
                        Phone = Feedback.Phone,
                        CreatedDate = Feedback.Create_Date?.ToString("yyyy-MM-dd"),
                        retrievedImages  = Feedback.Images?.Select(s => s.FilePath).ToList()
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
