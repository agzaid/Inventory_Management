﻿using Application.Common.Interfaces;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.Web.Mvc;

namespace Application.Services.Implementation
{
    public class DistrictService : IDistrictService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DistrictService> _logger;  // Inject ILogger<ShippingFrieghtService>

        public DistrictService(IUnitOfWork unitOfWork, ILogger<DistrictService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;  // Initialize the logger
        }

        public Result<IEnumerable<DistrictVM>> GetAllDistrics()
        {
            try
            {
                var district = _unitOfWork.District.GetAll(s => s.IsDeleted == false, "ShippingFreight");
                var allDistricts = district.Select(s => new DistrictVM()
                {
                    Id = s.Id,
                    Name = s.Name,
                    NameAr = s.NameAr,
                    AreaName= s.ShippingFreight?.ShippingArea,
                    Price = s.Price,
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                });

                _logger.LogInformation("GetAllShippingFrieght method completed. {allShippingFrieghtCount} allShippingFrieght retrieved.", allDistricts.ToList().Count);

                return Result<IEnumerable<DistrictVM>>.Success(allDistricts, "ShippingFrieght retrieved successfully."); ;
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
        public async Task<Result<string>> CreateDistrict(DistrictVM obj)
        {
            try
            {
                obj.Name = obj.Name?.ToLower();
                var lookForName = _unitOfWork.District.Get(s => s.Name == obj.Name);
                if (lookForName == null)
                {
                    var newFreight = new District()
                    {
                        Name = obj.Name,
                        NameAr = obj.NameAr,
                        ShippingFreightId = obj.AreaId,
                        Modified_Date = DateTime.Now,
                        Price = obj.Price,
                    };
                    _unitOfWork.District.Add(newFreight);
                    await _unitOfWork.Save();
                    return Result<string>.Success("new Freight Created Successfully", "Success");
                }
                else
                    return Result<string>.Failure("new Freight Already Exists", "error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating shippingFreight with Area: {Area}", obj.Name);
                return Result<string>.Failure("Error Occured...!!!", "error");  // Rethrow the exception after logging it
            }
        }

        public DistrictVM GetDistricById(int id)
        {
            try
            {
                var shipping = _unitOfWork.District.Get(u => u.Id == id);
                if (shipping != null)
                {
                    var shippingFrieghtVM = new DistrictVM()
                    {
                        Name = shipping.Name,
                        NameAr = shipping.NameAr,
                        AreaId = shipping.ShippingFreightId,
                        Price = shipping.Price,
                        CreatedDate = shipping.Create_Date?.ToString("yyyy-MM-dd"),
                        Areas = _unitOfWork.ShippingFreight.GetAll().Select(s=>new SelectListItem { 
                            Text=s.ShippingArea,
                            Value=s.Id.ToString()
                        }).ToList()
                    };
                    return shippingFrieghtVM;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving category with Id: {Id}", id);
                throw;  // Rethrow the exception after logging it
            }
            return new DistrictVM();
        }

        public async Task<bool> UpdateDistrict(DistrictVM obj)
        {
            try
            {
                obj.Name = obj.Name?.ToLower();
                var oldCategory = _unitOfWork.District.Get(s => s.Id == obj.Id);
                if (oldCategory != null)
                {
                    oldCategory.Name = obj.Name;
                    oldCategory.NameAr = obj.NameAr;
                    //oldCategory.Region= obj.Region;
                    oldCategory.Price = obj.Price;
                    oldCategory.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.District.Update(oldCategory);
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

        public async Task<bool> DeleteDistrict(int id)
        {
            try
            {
                var oldCategory = _unitOfWork.District.Get(s => s.Id == id);
                if (oldCategory != null)
                {
                    oldCategory.IsDeleted = true;
                    oldCategory.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.District.Update(oldCategory);
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

        public DistrictVM GetDistrictForCreateView()
        {
            try
            {
                var shippingFrieght = _unitOfWork.ShippingFreight.GetAll(s => s.IsDeleted == false);
                var district = new DistrictVM()
                {
                    Areas = shippingFrieght.Select(s => new SelectListItem
                    {
                        Text = s.ShippingArea,
                        Value = s.Id.ToString()
                    }).ToList(),
                };

                //_logger.LogInformation("GetAllShippingFrieght method completed. {allShippingFrieghtCount} allShippingFrieght retrieved.", allShippingFrieght.ToList().Count);

                return district;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving categories.");
                throw;  // Rethrow the exception after logging it
            }
        }
    }
}
