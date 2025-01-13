using Application.Common.Interfaces;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.Web.Mvc;

namespace Application.Services.Implementation
{
    public class ShippingFreightService : IShippingFreightService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ShippingFreightService> _logger;  // Inject ILogger<ShippingFrieghtService>

        public ShippingFreightService(IUnitOfWork unitOfWork, ILogger<ShippingFreightService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;  // Initialize the logger
        }

        public Result<IEnumerable<ShippingFreightVM>> GetAllShippingFrieght()
        {
            try
            {
                var shippingFrieght = _unitOfWork.ShippingFreight.GetAll(s => s.IsDeleted == false);
                var allShippingFrieght = shippingFrieght.Select(s => new ShippingFreightVM()
                {
                    Id = s.Id,
                    Area = s.Area,
                    Region = s.Region,
                    Price = s.Price,
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                });

                _logger.LogInformation("GetAllShippingFrieght method completed. {allShippingFrieghtCount} allShippingFrieght retrieved.", allShippingFrieght.ToList().Count);

                return Result<IEnumerable<ShippingFreightVM>>.Success(allShippingFrieght, "ShippingFrieght retrieved successfully."); ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving categories.");
                throw;  // Rethrow the exception after logging it
            }
        }

        public async Task<string> CreateShippingFrieghtForViewing(CategoryVM obj)
        {
            try
            {
                var productVM = new ProductVM();
                var category = _unitOfWork.Category.GetAll().ToList();
                productVM.ListOfCategory = category.Select(v => new SelectListItem
                {
                    Text = v.CategoryName,
                    Value = v.Id.ToString()
                }).ToList();
                return "productVM";
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<Result<string>> CreateShippingFreight(ShippingFreightVM obj)
        {
            try
            {
                obj.Area = obj.Area?.ToLower();
                obj.Region= obj.Region?.ToLower();
                var lookForName = _unitOfWork.ShippingFreight.Get(s => s.Area == obj.Area);
                if (lookForName == null)
                {
                    var newFreight = new ShippingFreight()
                    {
                        Area = obj.Area,
                        Modified_Date = DateTime.Now,
                        Region = obj.Region,
                        Price = obj.Price,
                    };
                    _unitOfWork.ShippingFreight.Add(newFreight);
                     _unitOfWork.Save();
                    return Result<string>.Success("new Freight Created Successfully","Success");
                }
                else
                    return Result<string>.Failure("new Freight Already Exists", "error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating shippingFreight with Area: {Area}", obj.Area);
                return Result<string>.Failure("Error Occured...!!!", "error");  // Rethrow the exception after logging it
            }
        }

        public ShippingFreightVM GetShippingFreightById(int id)
        {
            try
            {
                var shipping = _unitOfWork.ShippingFreight.Get(u => u.Id == id);
                if (shipping != null)
                {
                    var shippingFrieghtVM = new ShippingFreightVM()
                    {
                        Area = shipping.Area,
                        Region = shipping.Region,
                        Price = shipping.Price,
                        CreatedDate = shipping.Create_Date?.ToString("yyyy-MM-dd")
                    };
                    return shippingFrieghtVM;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving category with Id: {Id}", id);
                throw;  // Rethrow the exception after logging it
            }
            return new ShippingFreightVM();
        }

        public async Task<bool> UpdateShippingFreight(ShippingFreightVM obj)
        {
            try
            {
                obj.Area = obj.Area?.ToLower();
                obj.Region = obj.Region?.ToLower();
                var oldCategory = _unitOfWork.ShippingFreight.Get(s => s.Id == obj.Id);
                if (oldCategory != null)
                {
                    oldCategory.Area = obj.Area;
                    oldCategory.Region= obj.Region;
                    oldCategory.Price= obj.Price;
                    oldCategory.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.ShippingFreight.Update(oldCategory);
                     _unitOfWork.Save();
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

        public async Task<bool> DeleteShippingFreight(int id)
        {
            try
            {
                var oldCategory = _unitOfWork.ShippingFreight.Get(s => s.Id == id);
                if (oldCategory != null)
                {
                    oldCategory.IsDeleted = true;
                    oldCategory.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.ShippingFreight.Update(oldCategory);
                     _unitOfWork.Save();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting ShippingFreight with Id: {Id}", id);
                return false; // Rethrow the exception after logging it
            }
        }
    }
}
