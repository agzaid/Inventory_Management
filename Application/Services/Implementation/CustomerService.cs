using Application.Common.Interfaces;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.Web.Mvc;

namespace Application.Services.Implementation
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CustomerService> _logger;  // Inject ILogger<CategoryService>

        public CustomerService(IUnitOfWork unitOfWork, ILogger<CustomerService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;  // Initialize the logger
        }

        public Result<IEnumerable<CustomerVM>> GetAllCustomers()
        {
            try
            {
                var customers = _unitOfWork.Customer.GetAll(s => s.IsDeleted == false);
                var showCustomeres = customers.Select(s => new CustomerVM()
                {
                    Id = s.Id,
                    Phone = s.Phone,
                    Address = s.Address,
                    Area = s.Area,
                    CustomerName = s.CustomerName,
                    CustomerNameAr = s.CustomerNameAr,
                    Email = s.Email,
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                }).ToList();


                _logger.LogInformation("GetAllCustomers method completed. {CustomerCount} customers retrieved.", showCustomeres);

                return Result<IEnumerable<CustomerVM>>.Success(showCustomeres, "success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving customers.");
                return Result<IEnumerable<CustomerVM>>.Failure(ex.Message, "error");
                throw;  // Rethrow the exception after logging it
            }
        }
        public CustomerVM CreateCustomerForViewing()
        {
            try
            {
                var customerVM = new CustomerVM();
                var freights = _unitOfWork.ShippingFreight.GetAll().ToList();
                customerVM.ListOfAreas = freights.Select(s => new SelectListItem
                {
                    Text = s.ShippingArea,
                    Value = s.ShippingArea
                }).ToList();
                return customerVM;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<Result<string>> CreateCustomer(CustomerVM obj)
        {
            try
            {
                obj.CustomerName = obj.CustomerName?.ToLower();
                obj.Email = obj.Email?.ToLower();
                var lookForName = _unitOfWork.Customer.Get(s => s.Phone == obj.Phone);
                if (lookForName == null)
                {
                    var customer = new Customer()
                    {
                        CustomerName = obj.CustomerName,
                        CustomerNameAr = obj.CustomerNameAr,
                        Area = obj.Area,
                        Address = obj.Address,
                        Email = obj.Email,
                        Phone = obj.Phone,
                        Modified_Date = DateTime.Now,
                    };
                    _unitOfWork.Customer.Add(customer);
                    _unitOfWork.Save();
                    return Result<string>.Success("Customer Created Successfully", "success");
                }
                else
                    return Result<string>.Failure("Customer Already Exists", "error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating customer with CustomerName: {CustomerName}", obj.CustomerName);
                return Result<string>.Success("Error Occured...", "error");  // Rethrow the exception after logging it
            }
        }

        public Result<CustomerVM> GetCustomerById(int id)
        {
            try
            {
                var customer = _unitOfWork.Customer.Get(u => u.Id == id);
                if (customer != null)
                {
                    var customerVM = new CustomerVM()
                    {
                        CustomerName = customer.CustomerName,
                        CustomerNameAr = customer.CustomerNameAr,
                        AreaId = customer.Area,
                        Address = customer.Address,
                        Email = customer.Email,
                        Phone = customer.Phone,
                        CreatedDate = customer.Create_Date?.ToString("yyyy-MM-dd")
                    };
                    var customer1 = CreateCustomerForViewing();
                    customerVM.ListOfAreas = customer1.ListOfAreas;
                    return Result<CustomerVM>.Success(customerVM, "success");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving customer with Id: {Id}", id);
                throw;  // Rethrow the exception after logging it
            }
            return Result<CustomerVM>.Failure("something went wrong", "error");

        }

        public async Task<bool> UpdateCustomer(CustomerVM obj)
        {
            try
            {
                obj.CustomerName = obj.CustomerName?.ToLower();
                obj.Email = obj.Email?.ToLower();
                obj.Area = obj.Area?.ToLower();
                var oldCustomer = _unitOfWork.Customer.Get(s => s.Id == obj.Id);
                if (oldCustomer != null)
                {
                    oldCustomer.CustomerName = obj.CustomerName;
                    oldCustomer.CustomerNameAr = obj.CustomerNameAr;
                    oldCustomer.Area = obj.AreaId;
                    oldCustomer.Phone = obj.Phone;
                    oldCustomer.Email = obj.Email;
                   // oldCustomer.Area = obj.Area;
                    oldCustomer.Address = obj.Address;
                    oldCustomer.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.Customer.Update(oldCustomer);
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

        public async Task<bool> DeleteCustomer(int id)
        {
            try
            {
                var oldCustomer = _unitOfWork.Customer.Get(s => s.Id == id);
                if (oldCustomer != null)
                {
                    oldCustomer.IsDeleted = true;
                    oldCustomer.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.Customer.Update(oldCustomer);
                    _unitOfWork.Save();
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
