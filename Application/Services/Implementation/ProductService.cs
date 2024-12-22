using Application.Common.Interfaces;
using Application.Common.Utility;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Application.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<string> CreateProduct(ProductVM product)
        {
            try
            {
                var lookForName = _unitOfWork.Product.Get(s => s.ProductName == product.ProductName);
                if (lookForName != null)
                {
                    return "Product Already Exists";
                }
                else
                {
                    product.ProductName = product.ProductName?.ToLower();
                    product.Description = product.Description?.ToLower();

                    var result = await FileExtensions.CreateImages(product.ImagesFormFiles, product?.ProductName);
                    if (result != null)
                    {
                        var Newproduct = new Product()
                        {
                            ProductName = product.ProductName,
                            Description = product.Description,
                            Create_Date = DateTime.Now,
                            SellingPrice = product.SellingPrice,
                            BuyingPrice = product.BuyingPrice,
                            DifferencePercentage = product.DifferencePercentage,
                            IsDeleted = false,
                            MaximumDiscountPercentage = product.MaximumDiscountPercentage,
                            OtherShopsPrice = product.OtherShopsPrice,
                            StockQuantity = product.StockQuantity,
                            ProductExpiryDate = product.ExpiryDate,
                            Images = result.Select(s => new Image()
                            {
                                FilePath = s,
                                ImageName = s,
                                Create_Date = DateTime.Now,
                            }).ToList()
                        };
                        _unitOfWork.Product.Add(Newproduct);
                        await _unitOfWork.SaveAsync();
                    }
                    return "Product Created Successfully";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating product with ProductName: {ProductName}", product.ProductName);
                return "Error Occured...";  // Rethrow the exception after logging it
            }
        }

        public ProductVM CreateProductGetRequset()
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
                return productVM;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public bool DeleteProduct(int id)
        {
            try
            {
                var oldCategory = _unitOfWork.Category.Get(s => s.Id == id);
                if (oldCategory != null)
                {
                    oldCategory.IsDeleted = true;
                    oldCategory.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.Category.Update(oldCategory);
                    _unitOfWork.SaveAsync();
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

        public IEnumerable<ProductVM> GetAllProducts()
        {
            try
            {
                var products = _unitOfWork.Product.GetAll(s => s.IsDeleted == false);
                var showProducts = products.Select(s => new ProductVM()
                {
                    Id = s.Id,
                    Description = s.Description,
                    ProductName = s.ProductName,
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                }).ToList();

                _logger.LogInformation("Error Occured", showProducts.Count);

                return showProducts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving categories.");
                throw;  // Rethrow the exception after logging it
            }
        }

        public ProductVM GetProductById(int id)
        {
            try
            {
                var product = _unitOfWork.Product.Get(u => u.Id == id);
                if (product != null)
                {
                    var categoryVM = new ProductVM()
                    {
                        ProductName = product.ProductName,
                        Description = product.Description,
                        CreatedDate = product.Create_Date?.ToString("yyyy-MM-dd")
                    };
                    return categoryVM;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving category with Id: {Id}", id);
                throw;  // Rethrow the exception after logging it
            }
            return new ProductVM();
        }

        public bool UpdateProduct(ProductVM obj)
        {
            try
            {
                obj.ProductName = obj.ProductName?.ToLower();
                obj.Description = obj.Description?.ToLower();
                var oldProduct = _unitOfWork.Product.Get(s => s.Id == obj.Id);
                if (oldProduct != null)
                {
                    oldProduct.Description = obj.Description;
                    oldProduct.ProductName = obj.ProductName;
                    oldProduct.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.Product.Update(oldProduct);
                    _unitOfWork.SaveAsync();
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
    }
}
