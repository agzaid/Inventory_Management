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
using System.Globalization;
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

        public async Task<Result<string>> CreateProduct(ProductVM product)
        {
            try
            {
                // 1. Check duplicate product name
                var existingProduct = await _unitOfWork.Product.GetFirstOrDefaultAsync(
                    s => s.ProductName == product.ProductName.ToLower().Trim()
                );

                if (existingProduct != null)
                {
                    return Result<string>.Failure("Product Already Exists", "error");
                }

                // 2. Normalize fields
                product.ProductName = product.ProductName?.ToLower().Trim();
                product.ProductNameAr = product.ProductNameAr?.Trim();
                product.Description = product.Description?.ToLower().Trim();
                product.Brand = product.Brand?.ToLower().Trim();
                product.Seller = product.Seller?.ToLower().Trim();
                product.ProductTags = product.ProductTags?.ToLower().Trim();
                product.Barcode = product.Barcode?.ToLower().Trim();
                product.Slug = SlugGenerator.GenerateSlug(product.ProductName ?? "product");
                product.DifferencePercentage = product?.DifferencePercentage?.Replace("%", "").Trim();
                product.MaximumDiscountPercentage = product?.MaximumDiscountPercentage?.Replace("%", "").Trim();

                // 3. Handle image saving (just like Feedback)
                var imagesToBeAdded = new List<string>();
                if (product?.ImagesFormFiles?.Count > 0)
                {
                    foreach (var file in product.ImagesFormFiles)
                    {
                        var resultImagePath = await FileExtensions.SaveImageOptimized(file, "Products");
                        imagesToBeAdded.Add(resultImagePath);
                    }
                }

                var listOfImages = imagesToBeAdded.Select(path => new Domain.Entities.Image
                {
                    FilePath = path,
                    Create_Date = DateTime.Now,
                }).ToList();

                // 4. Create product entity
                var newProduct = new Product
                {
                    ProductName = product.ProductName,
                    ProductNameAr = product.ProductNameAr,
                    Description = product.Description,
                    Barcode = product.Barcode,
                    Slug = product.Slug,
                    Create_Date = DateTime.Now,
                    IsKilogram = product.IsKilogram,
                    PricePerGram = product.PricePerGram,
                    SellingPrice = product.SellingPrice,
                    BuyingPrice = product.BuyingPrice,
                    DifferencePercentage = decimal.TryParse(product.DifferencePercentage,
                                        NumberStyles.Any,
                                        CultureInfo.InvariantCulture,
                                        out var diff)
                        ? diff
                        : 0m,

                    MaximumDiscountPercentage = decimal.TryParse(product.MaximumDiscountPercentage,
                                             NumberStyles.Any,
                                             CultureInfo.InvariantCulture,
                                             out var maxDisc)
                            ? maxDisc
                            : 0m,
                    IsDeleted = false,
                    OtherShopsPrice = product.OtherShopsPrice,
                    StockQuantity = product.StockQuantity,
                    ProductExpiryDate = DateOnly.Parse(product.ExpiryDate ?? "2000-01-01"),
                    CategoryId = string.IsNullOrEmpty(product.CategoryId) ? null : int.Parse(product.CategoryId),
                    //oldProduct.BrandId = string.IsNullOrEmpty(obj.BrandId) ? null : int.Parse(obj.BrandId);

                    BrandId = string.IsNullOrEmpty(product.BrandId) ? null : int.Parse(product.BrandId),
                    SellerId = string.IsNullOrEmpty(product.SellerId) ? null : int.Parse(product.SellerId),
                    StatusId = (int?)(Status)Enum.Parse(typeof(Status), product.StatusId ?? ""),
                    ProductTags = product.ProductTags ?? "",
                    Images = listOfImages
                };

                // 5. Save to DB
                await _unitOfWork.Product.AddAsync(newProduct);
                await _unitOfWork.SaveAsync();

                return Result<string>.Success("success", "Product Created Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating product with ProductName: {ProductName}", product.ProductName);
                return Result<string>.Failure("Error Occured...", "error");
            }
        }


        public ProductVM CreateProductForViewingInCreate()
        {
            try
            {
                var productVM = new ProductVM();
                var category = _unitOfWork.Category.GetAll(s => s.IsDeleted == false).ToList();
                var brand = _unitOfWork.Brand.GetAll(s => s.IsDeleted == false).ToList();
                var seller = _unitOfWork.Seller.GetAll(s => s.IsDeleted == false).ToList();
                productVM.ListOfCategory = category.Select(v => new SelectListItem
                {
                    Text = v.CategoryName,
                    Value = v.Id.ToString()
                }).ToList();
                productVM.ListOfBrands = brand.Select(v => new SelectListItem
                {
                    Text = v.BrandName,
                    Value = v.Id.ToString()
                }).ToList();
                productVM.ListOfSellers = seller.Select(v => new SelectListItem
                {
                    Text = v.SellerName,
                    Value = v.Id.ToString()
                }).ToList();
                return productVM;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ProductVM CreateProductForViewingInCreate(ProductVM productVM)
        {
            try
            {
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

        public async Task<bool> DeleteProduct(int id)
        {
            try
            {
                var oldProduct = await _unitOfWork.Product.GetFirstOrDefaultAsync(s => s.Id == id);
                if (oldProduct != null)
                {
                    oldProduct.IsDeleted = true;
                    oldProduct.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.Product.Update(oldProduct);
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
        public async Task<bool> HardDeleteProduct(int id)
        {
            try
            {
                var oldProduct = await _unitOfWork.Product.GetFirstOrDefaultAsync(s => s.Id == id, "Images");
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

        public async Task<IEnumerable<ProductVM>> GetAllProducts()
        {
            try
            {
                var products = await _unitOfWork.Product.GetAllAsync(s => s.IsDeleted == false, "Category,Brand");
                var showProducts = products.Select(s => new ProductVM()
                {
                    Id = s.Id,
                    ProductName = s.ProductName?.ToUpper(),
                    ProductNameAr = s.ProductNameAr?.ToUpper(),
                    //Brand = s.Brand?.ToUpper(),
                    Description = s.Description,
                    Slug = s.Slug,
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
        public async Task<IEnumerable<ProductVM>> GetAllProductsForPortal()
        {
            var retrievedImages = new List<string>();
            var image64 = new List<string>();
            var productVMs = new List<ProductVM>();
            try
            {
                var products = await _unitOfWork.Product.GetAllAsync(s => s.IsDeleted == false, "Category,Images,Brand");

                //foreach (var item in products)
                //{
                //    retrievedImages.Clear();
                //    if (item.Images?.Count() > 0)
                //    {
                //        image64 = item.Images.Select(s => FileExtensions.ByteArrayToImageBase64(s.ImageByteArray)).ToList();
                //        retrievedImages.AddRange(image64);
                //    }
                //    var productVM = new ProductVM()
                //    {
                //        Id = item.Id,
                //        ProductName = item.ProductName?.ToUpper(),
                //        ProductNameAr = item.ProductNameAr,
                //        Description = item.Description,
                //        //Brand = item.Brand,
                //        CategoryName = item.Category?.CategoryName?.ToUpper(),
                //        SellingPrice = item.SellingPrice,
                //        IsKilogram = item.IsKilogram,
                //        PricePerGram = item.PricePerGram,
                //        StockQuantity = item.StockQuantity,
                //        ExpiryDate = item.ProductExpiryDate?.ToString("yyyy-MM-dd"),
                //        CreatedDate = item.Create_Date?.ToString("yyyy-MM-dd"),
                //        Barcode = item.Barcode,
                //        ListOfRetrievedImages = image64,
                //    };
                //    productVMs.Add(productVM);
                //}
                productVMs = products.Select(item => new ProductVM()
                {
                    Id = item.Id,
                    ProductName = item.ProductName?.ToUpper(),
                    ProductNameAr = item.ProductNameAr,
                    Description = item.Description,
                    Slug = item.Slug,
                    //Brand = item.Brand,
                    CategoryName = item.Category?.CategoryName?.ToUpper(),
                    SellingPrice = item.SellingPrice,
                    IsKilogram = item.IsKilogram,
                    PricePerGram = item.PricePerGram,
                    StockQuantity = item.StockQuantity,
                    ExpiryDate = item.ProductExpiryDate?.ToString("yyyy-MM-dd"),
                    CreatedDate = item.Create_Date?.ToString("yyyy-MM-dd"),
                    Barcode = item.Barcode,
                    ListOfRetrievedImages = item.Images?.Select(s => s.FilePath).ToList() ?? new List<string>()
                }).ToList();
                _logger.LogInformation("GetAllProducts method completed. {ProductCount} Products retrieved.", productVMs.Count);
                return productVMs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving categories.");
                throw;  // Rethrow the exception after logging it
            }
        }

        public async Task<ProductVM> GetProductById(int id)
        {
            try
            {
                var product = await _unitOfWork.Product.GetFirstOrDefaultAsync(u => u.Id == id, "Images,Brand");
                if (product != null)
                {
                    var productVM = new ProductVM()
                    {
                        ProductName = product.ProductName ?? "",
                        ProductNameAr = product.ProductNameAr ?? "",
                        Description = product.Description ?? "",
                        Slug = product.Slug ?? "",
                        //Brand = product.Brand ?? "",
                        Barcode = product.Barcode ?? "",
                        ExpiryDate = product.ProductExpiryDate?.ToString("yyyy-MM-dd") ?? "",
                        SellingPrice = product.SellingPrice ?? 0m,
                        IsKilogram = product.IsKilogram,
                        PricePerGram = product.PricePerGram ?? 0m,
                        BuyingPrice = product.BuyingPrice ?? 0m,
                        DifferencePercentage = product.DifferencePercentage?.ToString() ?? "",
                        MaximumDiscountPercentage = product.MaximumDiscountPercentage?.ToString() ?? "",
                        OtherShopsPrice = product.OtherShopsPrice ?? 0m,
                        StockQuantity = product.StockQuantity ?? 0,
                        CategoryId = product.CategoryId.ToString() ?? "",
                        BrandId = product.BrandId.ToString() ?? "",
                        SellerId = product.SellerId.ToString() ?? "",
                        StatusId = product.StatusId?.ToString() ?? "",
                        ProductTags = product.ProductTags ?? "",
                        ListOfRetrievedImages = product.Images?.Select(s => s.FilePath).ToList() ?? new List<string>(),
                    };
                    //if (product.Images?.Count() > 0)
                    //{
                    //    foreach (var item in product.Images)
                    //    {
                    //        //var s = FileExtensions.ByteArrayToImage(item.ImageByteArray);
                    //        if (item.ImageByteArray?.Length > 0)
                    //        {
                    //            var stringImages = FileExtensions.ByteArrayToImageBase64(item.ImageByteArray);
                    //            productVM.ListOfRetrievedImages?.Add(stringImages);
                    //        }
                    //    }
                    //}
                    var category = await _unitOfWork.Category.GetAllAsync(s => s.IsDeleted == false);
                    var brands = await _unitOfWork.Brand.GetAllAsync(s => s.IsDeleted == false);
                    var sellers = await _unitOfWork.Seller.GetAllAsync(s => s.IsDeleted == false);
                    productVM.ListOfCategory = category.Select(v => new SelectListItem
                    {
                        Text = v.CategoryName,
                        Value = v.Id.ToString()
                    }).ToList();
                    productVM.ListOfSellers = sellers.Select(v => new SelectListItem
                    {
                        Text = v.SellerName,
                        Value = v.Id.ToString()
                    }).ToList();
                    productVM.ListOfBrands = brands.Select(v => new SelectListItem
                    {
                        Text = v.BrandName,
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
        public async Task<ProductVM> GetProductDetails(int id)
        {
            try
            {
                var product = await GetProductById(id);
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving category with Id: {Id}", id);
                throw;  // Rethrow the exception after logging it
            }
        }
        public async Task<bool> UpdateProduct(ProductVM obj)
        {
            try
            {
                // Load old product with its images (tracked)
                var oldProduct = await _unitOfWork.Product.GetFirstOrDefaultAsync(
                    s => s.Id == obj.Id,
                    "Images",
                    true);

                if (oldProduct != null)
                {
                    // 1. Update product properties
                    oldProduct.ProductName = obj.ProductName?.ToLower().Trim();
                    oldProduct.ProductNameAr = obj.ProductNameAr?.Trim();
                    oldProduct.Description = obj.Description?.ToLower().Trim();
                    oldProduct.Barcode = obj.Barcode?.ToLower().Trim();
                    oldProduct.Slug = SlugGenerator.GenerateSlug(obj.ProductName);
                    oldProduct.Modified_Date = DateTime.UtcNow;
                    oldProduct.IsKilogram = obj.IsKilogram;
                    oldProduct.PricePerGram = obj.PricePerGram;
                    oldProduct.SellingPrice = obj.SellingPrice;
                    oldProduct.SellerId = string.IsNullOrEmpty(obj.SellerId) ? null : int.Parse(obj.SellerId);
                    oldProduct.BuyingPrice = obj.BuyingPrice;
                    oldProduct.DifferencePercentage = decimal.TryParse(obj.DifferencePercentage, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0m;
                    oldProduct.MaximumDiscountPercentage = decimal.TryParse(obj.MaximumDiscountPercentage, NumberStyles.Any, CultureInfo.InvariantCulture, out var m) ? m : 0m;

                    oldProduct.OtherShopsPrice = obj.OtherShopsPrice;
                    oldProduct.StockQuantity = obj.StockQuantity;
                    oldProduct.ProductExpiryDate = DateOnly.Parse(obj.ExpiryDate ?? "2000-01-01");
                    oldProduct.CategoryId = string.IsNullOrEmpty(obj.CategoryId) ? null : int.Parse(obj.CategoryId);
                    oldProduct.BrandId = string.IsNullOrEmpty(obj.BrandId) ? null : int.Parse(obj.BrandId);
                    oldProduct.StatusId = (int?)(Status)Enum.Parse(typeof(Status), obj.StatusId ?? "");
                    oldProduct.ProductTags = obj.ProductTags?.ToLower().Trim();

                    // 2. Remove unwanted old images
                    var imagesToBeRemoved = oldProduct.Images
                        .Where(s => !obj.OldImagesBytes.Contains(s.FilePath))
                        .ToList();

                    foreach (var img in imagesToBeRemoved)
                    {
                        _unitOfWork.Image.Remove(img); // remove from DB
                        await FileExtensions.DeleteImages(new List<string> { img.FilePath }); // remove from wwwroot
                    }

                    // 3. Add new uploaded images
                    if (obj.ImagesFormFiles?.Count > 0)
                    {
                        var resultImagePaths = await FileExtensions.SaveImagesOptimized(obj.ImagesFormFiles, "Products");

                        var newImages = resultImagePaths.Select(s => new Domain.Entities.Image
                        {
                            FilePath = s,
                            Create_Date = DateTime.Now,
                        }).ToList();

                        foreach (var img in newImages)
                        {
                            oldProduct.Images.Add(img);
                        }
                    }

                    // 4. Save changes
                    _unitOfWork.Product.Update(oldProduct);
                    await _unitOfWork.SaveAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating Product with Id: {Id}", obj.Id);
                return false;
            }
        }
        public async Task<int> UpdateAllSlugsAsync()
        {
            var products = await _unitOfWork.Category.GetAllAsync();

            foreach (var product in products)
            {
                var newSlug = SlugGenerator.GenerateSlug(product.CategoryName);

                if (product.Slug != newSlug)
                {
                    product.Slug = newSlug;
                    _unitOfWork.Category.Update(product);
                }
            }

            await _unitOfWork.SaveAsync();

            return products.Count();
        }
        public async Task<int> BulkUpdateProductsAsync(IEnumerable<Product> productVMs)
        {
            try
            {
                var ids = productVMs.Select(p => p.Id).ToList();

                var existingProducts = await _unitOfWork.Product
                    .GetAllAsync(p => ids.Contains(p.Id), tracked: true);

                // Map for quick lookup
                var productDict = existingProducts.ToDictionary(p => p.Id, p => p);

                int updatedCount = 0;

                foreach (var vm in productVMs)
                {
                    if (productDict.TryGetValue(vm.Id, out var product))
                    {
                        if (!string.IsNullOrWhiteSpace(vm.DisplayProductName) &&
                                 string.Equals(vm.DisplayProductName.Trim(), product.ProductName?.Trim(),
                           StringComparison.OrdinalIgnoreCase))
                        {
                            // --- Update fields safely ---
                            product.ProductName = vm.ProductName?.Trim().ToLower();
                            product.Description = vm.Description?.Trim().ToLower();
                            product.SellingPrice = vm.SellingPrice;
                            product.StockQuantity = (int)vm.StockQuantity;
                            // product.ProductExpiryDate = DateOnly.FromDateTime(vm.ExpiryDate);
                            product.Modified_Date = DateTime.UtcNow;

                            updatedCount++;
                        }
                        else
                        {
                            // Optional: log mismatch for reporting
                            _logger.LogWarning("Name mismatch for ID: {Id} | Expected: {Expected} | Got: {Got}",
                                vm.Id, product.ProductName, vm.DisplayProductName);
                            Console.WriteLine($"Name mismatch for ID: {vm.Id} | Expected: {product.ProductName} | Got: {vm.DisplayProductName}");
                            return 0;
                        }
                    }
                }

                await _unitOfWork.SaveAsync();

                return updatedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk update failed");
                return 0;
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
                    //var byteImage = FileExtensions.ConvertImageToByteArray(item);
                    byte[] byteImage = FileExtensions.ConvertImageToByteArray(item, 700, 90);
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
            oldProduct.ProductNameAr = obj.ProductNameAr;
            oldProduct.Description = obj.Description?.ToLower().Trim();
            oldProduct.Barcode = obj.Barcode?.ToLower().Trim();
            oldProduct.SellingPrice = obj.SellingPrice;
            oldProduct.IsKilogram = obj.IsKilogram;
            oldProduct.PricePerGram = obj.PricePerGram;
            oldProduct.BuyingPrice = obj.BuyingPrice;
            oldProduct.OtherShopsPrice = obj.OtherShopsPrice;
            oldProduct.StockQuantity = obj.StockQuantity;
            oldProduct.ProductExpiryDate = DateOnly.Parse(obj.ExpiryDate ?? "1-1-2000");
            oldProduct.CategoryId = string.IsNullOrEmpty(obj.CategoryId) ? null : int.Parse(obj.CategoryId);
            oldProduct.BrandId = string.IsNullOrEmpty(obj.BrandId) ? null : int.Parse(obj.BrandId);
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
