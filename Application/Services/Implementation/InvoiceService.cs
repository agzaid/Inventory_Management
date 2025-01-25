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
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(IUnitOfWork unitOfWork, ILogger<InvoiceService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<string[]> CreateInvoice(InvoiceVM invoiceVM)
        {
            try
            {
                var productQuantityMap = new Dictionary<int, int>();
                var customer = _unitOfWork.Customer.Get(s => s.Phone == invoiceVM.PhoneNumber);
                var invoice = new Invoice();
                var area = _unitOfWork.ShippingFreight.Get(s => s.Area == invoiceVM.shippingText);
                for (int i = 0; i < invoiceVM.productInput?.Count; i++)
                {
                    var product = _unitOfWork.Product.Get(s => s.ProductName == invoiceVM.productInput[i].ToLower());
                    var invoiceItems = new InvoiceItem()
                    {
                        ProductName = product.ProductName,
                        ProductId = product.Id,
                        Quantity = int.Parse(invoiceVM.quantityInput[i]),
                        Price = decimal.Parse(invoiceVM.priceInput[i]),
                        ShippingPrice = invoiceVM.shippingInput,
                        IndividualDiscount = double.Parse(invoiceVM.individualDiscount[i] == null ? "0" : invoiceVM.individualDiscount[i]),
                        // Product = product
                    };
                    if (productQuantityMap.ContainsKey(product.Id))
                    {
                        var s = invoice.InvoiceItems?.FirstOrDefault(s => s.ProductId == product.Id);
                        s.Quantity += int.Parse(invoiceVM.quantityInput[i]);
                        productQuantityMap[product.Id] += int.Parse(invoiceVM.quantityInput[i]); // Add quantity if product already exists
                    }
                    else
                    {
                        productQuantityMap[product.Id] = int.Parse(invoiceVM.quantityInput[i]); // Initialize the quantity if it's the first occurrence
                        invoice.InvoiceItems?.Add(invoiceItems);
                    }
                }

                foreach (var productEntry in productQuantityMap)
                {
                    var productUpdate = _unitOfWork.Product.Get(s => s.Id == productEntry.Key);
                    if (productUpdate != null)
                    {
                        productUpdate.StockQuantity -= productEntry.Value; // Update stock after all items are processed
                        _unitOfWork.Product.Update(productUpdate); // Update product only once
                    }
                }
                invoice.AreaId = area?.Id;
                //invoice.Customer = customer;
                invoice.CustomerId = customer?.Id;
                invoice.OrderDate = DateTime.UtcNow;
                invoice.AllDiscountInput = decimal.Parse(invoiceVM.allDiscountInput == null ? "0.00" : invoiceVM.allDiscountInput);
                invoice.GrandTotalAmount = invoiceVM.grandTotalInput;
                invoice.ProductsOnlyAmount = invoiceVM.totalAmountInput == 0 ? decimal.Parse("0.00") : (decimal)invoiceVM.totalAmountInput;
                invoice.AllProductItems = string.Join(',', invoiceVM.productInput);
                invoice.ShippingNotes = invoiceVM.ShippingNotes;
                invoice.ShippingPrice = double.Parse(invoiceVM.AreaId);

                //delete from quantity products

                _unitOfWork.Invoice.Add(invoice);
                _unitOfWork.Save();

                return new string[] { "success", "Invoice Created Successfully" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating invoice");
                //await FileExtensions.DeleteImages(imagesToBeDeleted);

                return new string[] { "error", "Error Occured..." };  // Rethrow the exception after logging it
            }
        }

        public Result<List<ProductVM>> SearchForProducts(string search)
        {
            try
            {
                var product = _unitOfWork.Product.GetAll(s => (s.ProductName.Contains(search) || s.Barcode.Contains(search)) && s.IsDeleted == false).ToList();
                if (product.Count == 0)
                {
                    return Result<List<ProductVM>>.Failure("Product not found...!!!", "error");
                }
                else if (product.Count > 0 && (product?.FirstOrDefault()?.StockQuantity == 0 || product?.FirstOrDefault()?.StockQuantity == null))
                {
                    return Result<List<ProductVM>>.Failure("Product Out of Stock...!!!", "error");
                }
                else
                {
                    var productViewModel = product.Select(s => new ProductVM
                    {
                        Id = s.Id,
                        ProductName = s.ProductName?.ToUpper(),
                        Description = s.Description,
                        CategoryName = s.Category?.CategoryName?.ToUpper(),
                        SellingPrice = s.SellingPrice,
                        DiscPerceForCreateInvoice = s.MaximumDiscountPercentage,
                        StockQuantity = s.StockQuantity,
                        CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                        Barcode = s.Barcode,
                        //ListOfRetrievedImages = s.Images?.Select(v => FileExtensions.ByteArrayToImageBase64(v.ImageByteArray)).ToList()
                    }).ToList();
                    return Result<List<ProductVM>>.Success(productViewModel, "Product retrieved successfully.");
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public Result<CustomerVM> SearchForCustomer(string search)
        {
            try
            {
                var customer = _unitOfWork.Customer.Get(s => s.Phone.Contains(search));
                if (customer == null)
                {
                    return Result<CustomerVM>.Failure("Customer not found...!!!", "error");
                }
                else
                {
                    var customerVm = new CustomerVM()
                    {
                        Phone = customer.Phone,
                        Address = customer.Address,
                        Area = customer.Area,
                        CustomerName = customer.CustomerName,
                        Email = customer.Email,
                        AreaId = customer.Area
                    };
                    return Result<CustomerVM>.Success(customerVm, "Customer retrieved successfully.");
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public InvoiceVM CreateInvoiceForViewing()
        {
            try
            {
                var productVM = new InvoiceVM();
                var freights = _unitOfWork.ShippingFreight.GetAll().ToList();
                productVM.ListOfAreas = freights.Select(v => new SelectListItem
                {
                    Text = v.Area,
                    Value = v.Price.ToString()
                }).ToList();
                return productVM;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public bool DeleteInvoice(int id)
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
        public bool HardDeleteInvoice(int id)
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

        public IEnumerable<ProductVM> GetAllInvoices()
        {
            try
            {
                var products = _unitOfWork.Product.GetAll(s => s.IsDeleted == false, "Category");
                var showProducts = products.Select(s => new ProductVM()
                {
                    Id = s.Id,
                    ProductName = s.ProductName?.ToUpper(),
                    CategoryName = s.Category?.CategoryName?.ToUpper(),
                    SellingPrice = s.SellingPrice,
                    StockQuantity = s.StockQuantity,
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

        public ProductVM GetInvoiceById(int id)
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
                    var category = _unitOfWork.Category.GetAll().ToList();
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
        public bool UpdateInvoice(ProductVM obj)
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
