using Application.Common.Interfaces;
using Application.Common.Utility;
using Application.Hubs;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InvoiceService> _logger;
        private readonly IHubContext<InventoryHub> _hub;

        public InvoiceService(IUnitOfWork unitOfWork, ILogger<InvoiceService> logger, IHubContext<InventoryHub> hub)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _hub = hub;
        }

        public IEnumerable<InvoiceVM> GetAllInvoices()
        {
            try
            {
                var invoices = _unitOfWork.Invoice.GetAll(s => s.IsDeleted == false, "InvoiceItems,Customer");
                var areas = _unitOfWork.District.GetAll(s => true, "ShippingFreight").ToList();

                var invoiceVMs = invoices.Select(s => new InvoiceVM
                {
                    Id = s.Id,
                    CustomerName = s.Customer?.CustomerName,
                    CustomerNameAr = s.Customer?.CustomerNameAr,
                    InvoiceNumber = s.InvoiceNumber,
                    TotalAmount = s.GrandTotalAmount,
                    PhoneNumber = s.Customer?.Phone,
                    AreaId = areas?.FirstOrDefault(a => a.Id == s.AreaId)?.Id,
                    shippingText = areas?.FirstOrDefault(a => a.Id == s.AreaId)?.Name,
                    ShippingNotes = s.ShippingNotes,
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                    AllProductsForIndexViewing = s.AllProductItems,
                }).ToList();

                _logger.LogInformation("GetAllInvoices method completed. {InvoiceCount} Invoices retrieved.", invoiceVMs.Count);

                return invoiceVMs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving Invoices.");
                throw;  // Rethrow the exception after logging it
            }
        }

        public async Task<string[]> CreateInvoice(InvoiceVM invoiceVM)
        {
            try
            {
                // ensure dictionary is initialized
                var productQuantityMap = new Dictionary<decimal, decimal>();

                // check customer existence
                var customer = await _unitOfWork.Customer.GetFirstOrDefaultAsync(s => s.Phone == invoiceVM.PhoneNumber);
                if (string.IsNullOrEmpty(invoiceVM.CustomerName) || string.IsNullOrEmpty(invoiceVM.PhoneNumber))
                {
                    return new string[] { "error", "Please provide us with phone number and name for customer" };
                }

                var existingInvoice = await _unitOfWork.Invoice.GetFirstOrDefaultAsync(s => s.InvoiceNumber == invoiceVM.InvoiceNumber);
                if (existingInvoice != null)
                {
                    return new string[] { "error", "Invoice already exists with this number." };
                }

                var invoice = new Invoice
                {
                    InvoiceItems = new List<InvoiceItem>()
                };

                var area = await _unitOfWork.District.GetFirstOrDefaultAsync(s => s.Id == invoiceVM.AreaId);

                for (int i = 0; i < invoiceVM.productInput?.Count; i++)
                {
                    var product = await _unitOfWork.Product.GetFirstOrDefaultAsync(s => s.ProductName == invoiceVM.productInput[i].ToLower());
                    if (product == null)
                    {
                        return new string[] { "error", $"Product '{invoiceVM.productInput[i]}' not found." };
                    }

                    decimal quantity = decimal.Parse(invoiceVM.quantityInput[i]);

                    var existingItem = invoice.InvoiceItems.FirstOrDefault(p => p.ProductId == product.Id);
                    if (existingItem != null)
                    {
                        existingItem.Quantity += quantity;
                        productQuantityMap[product.Id] += quantity;
                    }
                    else
                    {
                        productQuantityMap[product.Id] = quantity;
                        invoice.InvoiceItems.Add(new InvoiceItem
                        {
                            ProductId = product.Id,
                            ProductName = product.ProductName,
                            Quantity = quantity,
                            PriceSoldToCustomer = decimal.Parse(invoiceVM.priceInput[i]),
                            ShippingPrice = (decimal)invoiceVM.shippingInput,
                            IndividualDiscount = decimal.TryParse(invoiceVM.individualDiscount[i],
                                     NumberStyles.Any,
                                     CultureInfo.InvariantCulture,
                                     out var discount2)
                        ? discount2
                        : 0m

                        });
                    }
                }

                // stock updates
                var productUpdatesToBroadcast = new List<(decimal ProductId, decimal NewQty)>();

                foreach (var productEntry in productQuantityMap)
                {
                    var productUpdate = await _unitOfWork.Product.GetFirstOrDefaultAsync(s => s.Id == productEntry.Key);
                    if (productUpdate != null)
                    {
                        productUpdate.StockQuantity -= productEntry.Value;
                        _unitOfWork.Product.Update(productUpdate);

                        // collect to broadcast after save
                        productUpdatesToBroadcast.Add((Convert.ToDecimal(productUpdate.Id), (Convert.ToDecimal(productUpdate.StockQuantity))));
                    }
                }

                invoice.InvoiceNumber = invoiceVM.InvoiceNumber;
                invoice.AreaId = area?.Id;
                invoice.CustomerId = customer?.Id;
                invoice.CustomerName = invoiceVM.CustomerName;
                invoice.OrderDate = DateTime.UtcNow;
                invoice.AllDiscountInput = decimal.TryParse(invoiceVM.allDiscountInput,
                                            NumberStyles.Any,
                                            CultureInfo.InvariantCulture,
                                            out var discount) ? discount : 0m;
                invoice.GrandTotalAmount = (decimal)invoiceVM.grandTotalInput;
                invoice.ProductsOnlyAmount = invoiceVM.totalAmountInput == 0 ? 0 : (decimal)invoiceVM.totalAmountInput;
                invoice.AllProductItems = string.Join(',', invoiceVM.productInput);
                invoice.ShippingNotes = invoiceVM.ShippingNotes;
                invoice.ShippingPrice = area?.Price ?? 0m; // fixed

                await _unitOfWork.Invoice.AddAsync(invoice);

                var onlineOrder = await _unitOfWork.OnlineOrder.GetFirstOrDefaultAsync(s => s.OrderNumber == invoice.InvoiceNumber);
                if (onlineOrder != null)
                {
                    onlineOrder.OrderStatus = Status.ReadyToBeDelivered;
                    onlineOrder.Invoice = invoice;

                    _unitOfWork.OnlineOrder.Update(onlineOrder);
                }


                await _unitOfWork.SaveAsync();

                // Broadcast after commit so clients reflect persisted state
                foreach (var u in productUpdatesToBroadcast)
                {
                    try
                    {
                        await _hub.Clients.All.SendAsync("InventoryUpdated", u.ProductId, u.NewQty);
                        _logger.LogInformation("Broadcasted inventory update for product {ProductId} -> {Qty}", u.ProductId, u.NewQty);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed broadcasting inventory update for product {ProductId}", u.ProductId);
                    }
                }

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
                        IsKilogram = s.IsKilogram,
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
        public async Task<Result<CustomerVM>> SearchForCustomer(string search)
        {
            try
            {
                var customer = await _unitOfWork.Customer.GetFirstOrDefaultAsync(s => s.Phone.Contains(search));
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
                var freights = _unitOfWork.District.GetAll(s => s.IsDeleted == false, "ShippingFreight");
                productVM.ListOfAreas = freights.Select(v => new SelectListItem
                {
                    Text = $"{v.Id} - {v.Name} ({v.ShippingFreight?.ShippingArea}) ({v.Price})",
                    Value = v.Id.ToString()
                }).ToList();
                return productVM;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<InvoiceVM> GetInvoiceById()
        {
            try
            {
                var productVM = new InvoiceVM();
                var freights = await _unitOfWork.ShippingFreight.GetAllAsync();
                productVM.ListOfAreas = freights.Select(v => new SelectListItem
                {
                    Text = v.ShippingArea,
                    Value = v.Price.ToString()
                }).ToList();
                return productVM;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<bool> DeleteInvoice(int id)
        {
            try
            {
                var oldInvoice = await _unitOfWork.Invoice.GetFirstOrDefaultAsync(s => s.Id == id);
                if (oldInvoice != null)
                {
                    oldInvoice.IsDeleted = true;
                    oldInvoice.Modified_Date = DateTime.UtcNow;
                    _unitOfWork.Invoice.Update(oldInvoice);
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
        public async Task<bool> HardDeleteInvoice(int id)
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


        public InvoiceVM GetInvoiceById(int id)
        {
            try
            {
                var invoice = _unitOfWork.Invoice.Get(u => u.Id == id, "InvoiceItems,Customer");
                var freights = _unitOfWork.District.GetAll(s => s.IsDeleted == false, "ShippingFreight");
                if (invoice == null)
                {
                    return new InvoiceVM();
                }
                else
                {
                    var invoiceVM = new InvoiceVM()
                    {
                        Id = id,
                        CustomerName = invoice.Customer?.CustomerName,
                        InvoiceNumber = invoice.InvoiceNumber,
                        AllProductsForIndexViewing = invoice.AllProductItems,
                        allDiscountInput = invoice.AllDiscountInput.ToString(),
                        grandTotalInput = invoice.GrandTotalAmount,
                        totalAmountInput = invoice.ProductsOnlyAmount == null ? 0m : (decimal)invoice.ProductsOnlyAmount,
                        TotalAmount = invoice.ProductsOnlyAmount == null ? 0 : (decimal)invoice.ProductsOnlyAmount,
                        shippingInput = invoice.ShippingPrice,
                        AreaId = invoice.AreaId ?? 1,
                        individualDiscount = invoice.InvoiceItems?.Select(s => s.IndividualDiscount.ToString()).ToList(),
                        PhoneNumber = invoice.Customer?.Phone,
                        ShippingNotes = invoice.ShippingNotes,
                        productInput = invoice.InvoiceItems?.Select(s => s.ProductName).ToList(),
                        priceInput = invoice.InvoiceItems?.Select(s => s.PriceSoldToCustomer.ToString()).ToList(),
                        quantityInput = invoice.InvoiceItems?.Select(s => s.Quantity.ToString()).ToList(),
                        CreatedDate = invoice.Create_Date?.ToString("yyyy-MM-dd"),
                        ListOfAreas = freights.Select(v => new SelectListItem
                        {
                            Text = $"{v.Id} - {v.Name} ({v.ShippingFreight?.ShippingArea}) ({v.Price})",
                            Value = v.Id.ToString()
                        }).ToList()

                    };
                    invoiceVM.ListOfProductsVMs = _unitOfWork.Product
                         .GetAll(s => invoiceVM.productInput.Contains(s.ProductName)) // Directly filter based on product name matching any valueinproductInput
                         .Select(s => new ProductVM
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
                         })
                         .ToList();
                    return invoiceVM;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving category with Id: {Id}", id);
                throw;  // Rethrow the exception after logging it
            }
            return new InvoiceVM();
        }
        public async Task<bool> UpdateInvoice(ProductVM obj)
        {
            try
            {
                // Prepare byte arrays for images
                var imagesToBeInserted = new List<byte[]>();

                var oldProduct = await _unitOfWork.Product.GetFirstOrDefaultAsync(s => s.Id == obj.Id, "Images");

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
                    await _unitOfWork.SaveAsync();
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

        public async Task<List<MonthlyInventoryVM>> GetReportByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var invoiceItems = await _unitOfWork.InvoiceItem.GetAllAsync(ii => ii.Invoice != null
                          && ii.Invoice.OrderDate >= startDate
                          && ii.Invoice.OrderDate < endDate, includeProperties: "Invoice,Product", tracked: false);

            var report = invoiceItems
                     .GroupBy(ii => ii.ProductId)
                     .Select(g => new MonthlyInventoryVM
                     {
                         ProductId = g.Key ?? 0,
                         ProductName = g.First().ProductName,
                         TotalQuantitySold = g.Sum(ii => ii.Quantity ?? 0),
                         TotalRevenue = g.Sum(ii => (ii.PriceSoldToCustomer ?? 0) * (ii.Quantity ?? 0)),
                         TotalDiscount = g.Sum(ii => (decimal)(ii.IndividualDiscount ?? 0)),
                         RemainingStock = g.First().Product?.StockQuantity ?? 0,
                         StartDate = startDate,
                         EndDate = endDate
                     })
                     .ToList();

            return report;
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
            oldProduct.Description = obj.Description?.ToLower().Trim();
            oldProduct.Barcode = obj.Barcode?.ToLower().Trim();
            oldProduct.SellingPrice = obj.SellingPrice;
            oldProduct.BuyingPrice = obj.BuyingPrice;
            oldProduct.OtherShopsPrice = obj.OtherShopsPrice;
            oldProduct.StockQuantity = obj.StockQuantity;
            oldProduct.ProductExpiryDate = DateOnly.Parse(obj.ExpiryDate ?? "1-1-2000");
            oldProduct.CategoryId = string.IsNullOrEmpty(obj.CategoryId) ? null : int.Parse(obj.CategoryId);
            oldProduct.StatusId = (int?)(Status)Enum.Parse(typeof(Status), obj.StatusId ?? "");
            oldProduct.ProductTags = obj.ProductTags?.ToLower().Trim(); oldProduct.DifferencePercentage = decimal.TryParse(
                obj?.DifferencePercentage?.Replace("%", "").Trim(),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var diff
            ) ? diff : 0m;

            oldProduct.MaximumDiscountPercentage = decimal.TryParse(
                obj?.MaximumDiscountPercentage?.Replace("%", "").Trim(),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var maxDisc
            ) ? maxDisc : 0m;

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
