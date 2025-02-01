using Application.Common.Interfaces;
using Application.Common.Utility;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;

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

        public async Task<string[]> CreateProduct(ProductVM product)
        {
            var imagesToBeDeleted = new List<string>();
            var imagesToBeRemoved = new List<byte[]>();
            try
            {
                var lookForName = _unitOfWork.Product.Get(s => s.ProductName == product.ProductName.ToLower().Trim());
                if (lookForName != null)
                {
                    return new string[] { "error", "Product Already Exists" };
                }
                else
                {
                    product.ProductName = product.ProductName?.ToLower().Trim();
                    product.Description = product.Description?.ToLower().Trim();
                    product.ProductTags = product.ProductTags?.ToLower().Trim();
                    product.Barcode = product.Barcode?.ToLower().Trim();
                    product.DifferencePercentage = product?.DifferencePercentage?.Replace("%", "").Trim();
                    product.MaximumDiscountPercentage = product?.MaximumDiscountPercentage?.Replace("%", "").Trim();
                    var result = new List<string>();
                    var resultByteImage = new byte[0];
                    if (product?.ImagesFormFiles?.Count() > 0)
                    {
                        //result = await FileExtensions.CreateImages(product.ImagesFormFiles, product?.ProductName);
                        foreach (var item in product.ImagesFormFiles)
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

                    if (!(result == null && result.Contains("Error")))
                    {
                        var Newproduct = new Product()
                        {
                            ProductName = product.ProductName,
                            Description = product.Description,
                            Barcode = product.Barcode,
                            Create_Date = DateTime.Now,
                            SellingPrice = product.SellingPrice,
                            BuyingPrice = product.BuyingPrice,
                            DifferencePercentage = decimal.Parse(product.DifferencePercentage ?? "0.00"),
                            IsDeleted = false,
                            MaximumDiscountPercentage = decimal.Parse(product.MaximumDiscountPercentage ?? "0.00"),
                            OtherShopsPrice = product.OtherShopsPrice,
                            StockQuantity = product.StockQuantity,
                            ProductExpiryDate = DateOnly.Parse(product.ExpiryDate ?? "1-1-2000"),
                            CategoryId = int.Parse(product.CategoryId),
                            StatusId = (int?)(Status)Enum.Parse(typeof(Status), product.StatusId ?? ""),
                            ProductTags = product.ProductTags ?? "",
                            Images = listOfImages,
                        };
                        _unitOfWork.Product.Add(Newproduct);
                        _unitOfWork.Save();
                    }
                    return new string[] { "success", "Product Created Successfully" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating product with ProductName: {ProductName}", product.ProductName);
                //await FileExtensions.DeleteImages(imagesToBeDeleted);

                return new string[] { "error", "Error Occured..." };  // Rethrow the exception after logging it
            }
        }

        public ProductVM CreateProductForViewingInCreate()
        {
            try
            {
                var productVM = new ProductVM();
                var category = _unitOfWork.Category.GetAll(s => s.IsDeleted == false).ToList();
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
                var oldProduct = _unitOfWork.Product.Get(s => s.Id == id);
                if (oldProduct != null)
                {
                    oldProduct.IsDeleted = true;
                    oldProduct.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.Product.Update(oldProduct);
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
        public bool HardDeleteProduct(int id)
        {
            try
            {
                var oldProduct = _unitOfWork.Product.Get(s => s.Id == id, "Images");
                if (oldProduct != null)
                {
                    if (oldProduct.Images?.Count > 0)
                    {
                        foreach (var item in oldProduct.Images)
                        {
                            _unitOfWork.Image.Remove(item);
                        }
                    }
                    _unitOfWork.Product.Remove(oldProduct);
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

        public IEnumerable<ProductVM> GetAllProducts()
        {
            try
            {
                var products = _unitOfWork.Product.GetAll(s => s.IsDeleted == false, "Category");
                var showProducts = products.Select(s => new ProductVM()
                {
                    Id = s.Id,
                    ProductName = s.ProductName?.ToUpper(),
                    Description = s.Description,
                    CategoryName = s.Category?.CategoryName?.ToUpper(),
                    SellingPrice = s.SellingPrice,
                    StockQuantity = s.StockQuantity,
                    ExpiryDate = s.ProductExpiryDate?.ToString("yyyy-MM-dd"),
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                    Barcode = s.Barcode,
                }).ToList();

                _logger.LogInformation("GetAllProducts method completed. {ProductCount} Products retrieved.", showProducts.Count);
                return showProducts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving categories.");
                throw;  // Rethrow the exception after logging it
            }
        }
        public IEnumerable<ProductVM> GetAllProductsForPortal()
        {
            var retrievedImages = new List<string>();
            var image64 = new List<string>();
            var productVMs = new List<ProductVM>();
            try
            {
                var products = _unitOfWork.Product.GetAll(s => s.IsDeleted == false, "Category,Images");

                foreach (var item in products)
                {
                    retrievedImages.Clear();
                    if (item.Images?.Count() > 0)
                    {
                        image64 = item.Images.Select(s => FileExtensions.ByteArrayToImageBase64(s.ImageByteArray)).ToList();
                        retrievedImages.AddRange(image64);
                    }
                    var productVM = new ProductVM()
                    {
                        Id = item.Id,
                        ProductName = item.ProductName?.ToUpper(),
                        Description = item.Description,
                        CategoryName = item.Category?.CategoryName?.ToUpper(),
                        SellingPrice = item.SellingPrice,
                        StockQuantity = item.StockQuantity,
                        ExpiryDate = item.ProductExpiryDate?.ToString("yyyy-MM-dd"),
                        CreatedDate = item.Create_Date?.ToString("yyyy-MM-dd"),
                        Barcode = item.Barcode,
                        ListOfRetrievedImages = image64,
                    };
                    productVMs.Add(productVM);
                }
                _logger.LogInformation("GetAllProducts method completed. {ProductCount} Products retrieved.", productVMs.Count);
                return productVMs;
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
                var product = _unitOfWork.Product.Get(u => u.Id == id, "Images");
                if (product != null)
                {
                    var productVM = new ProductVM()
                    {
                        ProductName = product.ProductName ?? "",
                        Description = product.Description ?? "",
                        Barcode = product.Barcode ?? "",
                        ExpiryDate = product.ProductExpiryDate?.ToString("yyyy-MM-dd") ?? "",
                        SellingPrice = product.SellingPrice ?? decimal.Parse("0.00"),
                        BuyingPrice = product.BuyingPrice ?? decimal.Parse("0.00"),
                        DifferencePercentage = product.DifferencePercentage?.ToString() ?? "",
                        MaximumDiscountPercentage = product.MaximumDiscountPercentage?.ToString() ?? "",
                        OtherShopsPrice = product.OtherShopsPrice ?? decimal.Parse("0.00"),
                        StockQuantity = product.StockQuantity ?? int.Parse("0"),
                        CategoryId = product.CategoryId.ToString() ?? "",
                        StatusId = product.StatusId?.ToString() ?? "",
                        ProductTags = product.ProductTags ?? "",
                        //Images = result.Select(s => new Image()
                        //{
                        //    FilePath = s,
                        //    ImageName = s,
                        //    Create_Date = DateTime.Now,
                        //}).ToList()
                    };
                    if (product.Images?.Count() > 0)
                    {
                        foreach (var item in product.Images)
                        {
                            //var s = FileExtensions.ByteArrayToImage(item.ImageByteArray);
                            if (item.ImageByteArray?.Length > 0)
                            {
                                var stringImages = FileExtensions.ByteArrayToImageBase64(item.ImageByteArray);
                                productVM.ListOfRetrievedImages?.Add(stringImages);
                            }
                        }
                    }
                    var category = _unitOfWork.Category.GetAll(s => s.IsDeleted == false).ToList();
                    productVM.ListOfCategory = category.Select(v => new SelectListItem
                    {
                        Text = v.CategoryName,
                        Value = v.Id.ToString()
                    }).ToList();
                    return productVM;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving category with Id: {Id}", id);
                throw;  // Rethrow the exception after logging it
            }
            return new ProductVM();
        }
        public ProductVM GetProductDetails(int id)
        {
            try
            {
                var product = GetProductById(id);
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving category with Id: {Id}", id);
                throw;  // Rethrow the exception after logging it
            }
        }
        public bool UpdateProduct(ProductVM obj)
        {
            try
            {
                // Prepare byte arrays for images
                var imagesToBeInserted = new List<byte[]>();

                var oldProduct = _unitOfWork.Product.Get(s => s.Id == obj.Id, "Images");

                // Remove old images if necessary
                RemoveOldImages(oldProduct);

                // Add new images from form files or old image bytes
                AddNewImages(obj.ImagesFormFiles, obj.OldImagesBytes, imagesToBeInserted);

                // Create Image entities from byte arrays
                var listOfImages = CreateImageEntities(imagesToBeInserted);

                // Update product properties
                if (oldProduct != null)
                {
                    UpdateProductProperties(oldProduct, obj, listOfImages);

                    // Save the updated product
                    _unitOfWork.Product.Update(oldProduct);
                    _unitOfWork.Save();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating product with Id: {Id}", obj.Id);
                return false;
            }
        }


        #region Update Product Private Methods

        // Method to remove old images from the product
        private void RemoveOldImages(Product oldProduct)
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

        // Method to update product properties
        private void UpdateProductProperties(Product oldProduct, ProductVM obj, List<Domain.Entities.Image> listOfImages)
        {
            oldProduct.ProductName = obj.ProductName?.ToLower().Trim();
            oldProduct.Description = obj.Description?.ToLower().Trim();
            oldProduct.Barcode = obj.Barcode?.ToLower().Trim();
            oldProduct.SellingPrice = obj.SellingPrice;
            oldProduct.BuyingPrice = obj.BuyingPrice;
            oldProduct.OtherShopsPrice = obj.OtherShopsPrice;
            oldProduct.StockQuantity = obj.StockQuantity;
            oldProduct.ProductExpiryDate = DateOnly.Parse(obj.ExpiryDate ?? "1-1-2000");
            oldProduct.CategoryId = int.Parse(obj.CategoryId ?? "0");
            oldProduct.StatusId = (int?)(Status)Enum.Parse(typeof(Status), obj.StatusId ?? "");
            oldProduct.ProductTags = obj.ProductTags?.ToLower().Trim();
            oldProduct.DifferencePercentage = decimal.Parse(obj?.DifferencePercentage?.Replace("%", "").Trim() ?? "0.00");
            oldProduct.MaximumDiscountPercentage = decimal.Parse(obj?.MaximumDiscountPercentage?.Replace("%", "").Trim() ?? "0.00");
            oldProduct.Modified_Date = DateTime.UtcNow;
            oldProduct.Images = listOfImages;
        }


        //public bool UpdateProduct(ProductVM obj)
        //{
        //    try
        //    {
        //        var resultByteImage = new byte[0];
        //        var imagesToBeRemoved = new List<byte[]>();
        //        var imagesToBeInserted = new List<byte[]>();
        //        var oldImages = obj.OldImagesBytes;

        //        var oldProduct = _unitOfWork.Product.Get(s => s.Id == obj.Id, "Images");

        //        // Remove old images
        //        if (oldProduct?.Images?.Count > 0)
        //        {
        //            _logger.LogInformation("Removing old images for product with Id: {Id}", obj.Id);
        //            foreach (var item in oldProduct.Images)
        //            {
        //                _unitOfWork.Image.Remove(item);
        //            }
        //            oldProduct.Images.Clear();
        //            _logger.LogInformation("Old images removed.");
        //        }

        //        // Add new images from form files
        //        if (obj.ImagesFormFiles?.Count > 0)
        //        {
        //            foreach (var item in obj.ImagesFormFiles)
        //            {
        //                resultByteImage = FileExtensions.ConvertImageToByteArray(item);
        //                imagesToBeInserted.Add(resultByteImage);
        //            }
        //        }

        //        // Add old images (if any)
        //        if (obj.OldImagesBytes?.Count > 0)
        //        {
        //            foreach (var item in obj.OldImagesBytes)
        //            {
        //                var newImagesBytes = FileExtensions.FromImageToByteArray(item);
        //                imagesToBeInserted.Add(newImagesBytes);
        //            }
        //        }

        //        // Create new Image entities
        //        var listOfImages = imagesToBeInserted.Select(s => new Domain.Entities.Image()
        //        {
        //            ImageByteArray = s ?? new byte[0],
        //            Create_Date = DateTime.Now,
        //        }).ToList();

        //        // Update product
        //        if (oldProduct != null)
        //        {
        //            oldProduct.ProductName = obj.ProductName?.ToLower().Trim();
        //            oldProduct.Description = obj.Description?.ToLower().Trim();
        //            oldProduct.SellingPrice = obj.SellingPrice;
        //            oldProduct.BuyingPrice = obj.BuyingPrice;
        //            oldProduct.OtherShopsPrice = obj.OtherShopsPrice;
        //            oldProduct.StockQuantity = obj.StockQuantity;
        //            oldProduct.ProductExpiryDate = DateOnly.Parse(obj.ExpiryDate ?? "1-1-2000");
        //            oldProduct.CategoryId = int.Parse(obj.CategoryId ?? "0");
        //            oldProduct.StatusId = (int?)(Status)Enum.Parse(typeof(Status), obj.StatusId ?? "");
        //            oldProduct.ProductTags = obj.ProductTags?.ToLower().Trim();
        //            oldProduct.DifferencePercentage = decimal.Parse(obj?.DifferencePercentage?.Replace("%", "").Trim() ?? "0.00");
        //            oldProduct.MaximumDiscountPercentage = decimal.Parse(obj?.MaximumDiscountPercentage?.Replace("%", "").Trim() ?? "0.00");
        //            oldProduct.ProductTags = obj.ProductTags?.ToLower().Trim();
        //            oldProduct.Modified_Date = DateTime.UtcNow;
        //            oldProduct.Images = listOfImages;

        //            _unitOfWork.Product.Update(oldProduct);
        //            _unitOfWork.Save();
        //            return true;
        //        }
        //        else
        //            return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while updating product with Id: {Id}", obj.Id);
        //        return false;  // Rethrow the exception after logging it
        //    }

        //}
        #endregion
    }
}
