using Application.Common.Interfaces;
using Application.Common.Utility;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Application.Services.Implementation
{
    public class OnlineOrderService : IOnlineOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OnlineOrderService> _logger;

        public OnlineOrderService(IUnitOfWork unitOfWork, ILogger<OnlineOrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public PortalVM GetAllProductsForPortal()
        {
            var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            var retrievedImages = new List<string>();
            var image64 = new List<string>();
            var productVMs = new List<ProductVM>();
            var portalVM = new PortalVM();
            try
            {
                var brands = _unitOfWork.Brand.GetAll(s => s.IsDeleted == false, "Images").ToList();
                var products = _unitOfWork.Product.GetAll(s => s.IsDeleted == false, "Category,Images");
                var categories = _unitOfWork.Category.GetAll(s => s.IsDeleted == false).ToList();
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
                        OtherShopsPrice = item.OtherShopsPrice,
                        DifferencePercentage = Math.Ceiling(item.DifferencePercentage ?? 0).ToString("0.00") ?? "0.00",
                        StockQuantity = item.StockQuantity,
                        ExpiryDate = item.ProductExpiryDate?.ToString("yyyy-MM-dd"),
                        CreatedDate = item.Create_Date?.ToString("yyyy-MM-dd"),
                        Barcode = item.Barcode,
                        ListOfRetrievedImages = image64,
                    };
                    productVMs.Add(productVM);
                }
                portalVM.ProductVMs = productVMs;
                portalVM.CategoryVMs = categories.Select(s => new CategoryVM
                {
                    Id = s.Id,
                    CategoryName = s.CategoryName?.ToUpper(),
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                    Description = s.Description,
                }).ToList();

                _logger.LogInformation("GetAllProducts method completed. {ProductCount} Products retrieved.", productVMs.Count);
                return portalVM;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex, ex.InnerException.Message);
                }
                else
                    _logger.LogError(ex, ex.Message);
                return new PortalVM();
                //throw;  // Rethrow the exception after logging it
            }
        }
        public Task<Result<List<ProductVM>>> GetProductsByCategory(int? categoryId)
        {
            try
            {
                var retrievedImages = new List<string>();
                var image64 = new List<string>();
                var productVMs = new List<ProductVM>();

                var products = _unitOfWork.Product.GetAll(s => s.IsDeleted == false && (categoryId == null || s.CategoryId == categoryId), "Category,Images");
                var categories = _unitOfWork.Category.GetAll(s => s.IsDeleted == false).ToList();
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
                        OtherShopsPrice = item.OtherShopsPrice,
                        DifferencePercentage = Math.Ceiling(item.DifferencePercentage ?? 0).ToString("0.00") ?? "0.00",
                        StockQuantity = item.StockQuantity,
                        ExpiryDate = item.ProductExpiryDate?.ToString("yyyy-MM-dd"),
                        CreatedDate = item.Create_Date?.ToString("yyyy-MM-dd"),
                        Barcode = item.Barcode,
                        ListOfRetrievedImages = image64,
                    };
                    productVMs.Add(productVM);
                }

                return Task.FromResult(Result<List<ProductVM>>.Success(productVMs, "success"));
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex, ex.InnerException.Message);
                }
                else
                {
                    _logger.LogError(ex, ex.Message);
                }
                return Task.FromResult(Result<List<ProductVM>>.Failure("Failed", "error"));
            }
        }
        public Task<Result<List<ProductVM>>> GetProductsByName(string? name)
        {
            try
            {
                var retrievedImages = new List<string>();
                var image64 = new List<string>();
                var productVMs = new List<ProductVM>();

                var products = _unitOfWork.Product.GetAll(s => s.IsDeleted == false && s.ProductName.Contains(name), "Category,Images");
                var categories = _unitOfWork.Category.GetAll(s => s.IsDeleted == false).ToList();
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
                        OtherShopsPrice = item.OtherShopsPrice,
                        DifferencePercentage = Math.Ceiling(item.DifferencePercentage ?? 0).ToString("0.00") ?? "0.00",
                        StockQuantity = item.StockQuantity,
                        ExpiryDate = item.ProductExpiryDate?.ToString("yyyy-MM-dd"),
                        CreatedDate = item.Create_Date?.ToString("yyyy-MM-dd"),
                        Barcode = item.Barcode,
                        ListOfRetrievedImages = image64,
                    };
                    productVMs.Add(productVM);
                }

                return Task.FromResult(Result<List<ProductVM>>.Success(productVMs, "success"));
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex, ex.InnerException.Message);
                }
                else
                {
                    _logger.LogError(ex, ex.Message);
                }
                return Task.FromResult(Result<List<ProductVM>>.Failure("Failed", "error"));
            }
        }
        public async Task<Result<PaginatedResult<ProductVM>>> GetProductsPaginated(int pageNumber, int pageSize)
        {
            try
            {
                Expression<Func<Product, bool>> filter = s => s.IsDeleted == false;
                Expression<Func<Product, object>> includes = x => x.Images;
                Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy;
                orderBy = s => s.OrderByDescending(s => s.ProductName);

                var products = await _unitOfWork.Product.GetPaginatedAsync(pageNumber, pageSize, orderBy, filter, includes);

                var showProducts = products.Items.Select(s => new ProductVM()
                {
                    Id = s.Id,
                    ProductName = s.ProductName?.ToUpper(),
                    Description = s.Description,
                    CategoryName = s.Category?.CategoryName?.ToUpper(),
                    SellingPrice = s.SellingPrice,
                    OtherShopsPrice = s.OtherShopsPrice,
                    DifferencePercentage = Math.Ceiling(s.DifferencePercentage ?? 0).ToString("0.00") ?? "0.00",
                    StockQuantity = s.StockQuantity,
                    ExpiryDate = s.ProductExpiryDate?.ToString("yyyy-MM-dd"),
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                    Barcode = s.Barcode,
                    ListOfRetrievedImages = s.Images?.Select(a => FileExtensions.ByteArrayToImageBase64(a.ImageByteArray)).ToList(),
                }).ToList();
                var paginatedResult = new PaginatedResult<ProductVM>
                {
                    Items = showProducts,
                    TotalCount = products.TotalCount,
                    PageNumber = products.PageNumber,
                    PageSize = products.PageSize
                };
                return Result<PaginatedResult<ProductVM>>.Success(paginatedResult, "success");
                // return paginatedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Getting products paginated");
                throw;
            }

        }
        public async Task<List<SelectListItem>> ShippingFreightSelectList()
        {
            try
            {
                var shipping = _unitOfWork.ShippingFreight.GetAll(s => s.IsDeleted == false);
                var vm = shipping.Select(s => new SelectListItem()
                {
                    Text = $"{s.ShippingArea} ({s.Price})",
                    Value = s.Price.ToString()
                }).ToList();
                return vm;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex, ex.InnerException.Message);
                }
                else
                    _logger.LogError(ex, ex.Message);
                return new List<SelectListItem>();
            }

        }
        public async Task<List<DeliverySlotVM>> DeliverySlot()
        {
            try
            {
                var deliverySlot = _unitOfWork.DeliverySlot.GetAll().OrderBy(s => s.StartTime).ToList();
                var vm = deliverySlot.Select(s => new DeliverySlotVM()
                {
                    AM_PM = s.AM_PM?.ToUpper(),
                    EndTime = s.EndTime,
                    StartTime = s.StartTime,
                    IsAvailable = s.IsAvailable,
                }).ToList();
                return vm;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex, ex.InnerException.Message);
                }
                else
                    _logger.LogError(ex, ex.Message);

                return null;
            }

        }
        public async Task<Result<string>> CreateOrder(CartVM cart)
        {
            try
            {
                if (cart == null)
                {
                    return Result<string>.Failure("Cart is null", "error");
                }
                else
                {
                    ShippingFreight shipping = null;
                    IEnumerable<DeliverySlot> deliverySlot;
                    double shippingPrice = 0;
                    var grandTotalPrice = 0;
                    if (!string.IsNullOrWhiteSpace(cart.ShippingAreaPrice) && double.TryParse(cart.ShippingAreaPrice, out var parsedShippingPrice))
                    {
                        shippingPrice = parsedShippingPrice;
                        shipping = _unitOfWork.ShippingFreight.Get(s => s.Price == parsedShippingPrice);
                    }
                    var customer = _unitOfWork.Customer.Get(s => s.Phone == cart.CustomerPhone);
                    if (customer == null)
                    {
                        var newCustomer = new Customer
                        {
                            CustomerName = cart.CustomerName,
                            Address = cart.CustomerAddress,
                            Phone = cart.CustomerPhone,
                        };
                        _unitOfWork.Customer.Add(newCustomer);
                        customer = newCustomer;
                    }
                    var userDeliverySlot = new List<UserDeliverySlot>();
                    if (cart.SelectedSlots?.Length > 0)
                    {
                        var deliverySlotVM = cart.SelectedSlots.Select(s => s.Split('-')[0].Trim());
                        deliverySlot = _unitOfWork.DeliverySlot.GetAll(s => deliverySlotVM.Contains(s.StartTime));
                        foreach (var item in deliverySlot)
                        {

                            //item.UserDeliverySlots ??= new List<UserDeliverySlot>();
                            userDeliverySlot.Add(new UserDeliverySlot()
                            {
                                DeliverySlotId = item.Id,
                                Customer = customer,
                                //CustomerId = customer.Id
                                //DeliverySlot = item,
                                //UserId = customer.Id,
                            });
                        }
                    }

                    var onlineOrder = new OnlineOrder()
                    {
                        IndividualProductsNames = string.Join(", ", cart.ItemsVMs.Select(s => s.ProductName)),
                        IndividualProductsPrices = string.Join(", ", cart.ItemsVMs.Select(s => s.ProductPrice)),
                        IndividualProductsQuatities = string.Join(", ", cart.ItemsVMs.Select(s => s.Quantity)),
                        GrandTotalAmount = cart.TotalPrice,
                        AmountBeforeShipping = cart.PriceBeforeShipping,
                        CustomerId = customer.Id,
                        ShippingPrice = double.Parse(cart.ShippingAreaPrice ?? "0"),
                        ShippingNotes = cart.CustomerAddress,
                        DeliverySlotsAsString = cart.SelectedSlots,
                        //  UserDeliverySlots = userDeliverySlot,
                        OrderStatus = Status.InProgress,
                        OrderDate = DateTime.Now,
                        AreaId = shipping?.Id,
                        AllDiscountInput = 0,
                        InvoiceId = 0
                    };


                    foreach (var item in cart.ItemsVMs)
                    {
                        var product = _unitOfWork.Product.Get(s => s.Id == item.ProductId);
                        if (product != null)
                        {
                            var invoiceItem = new InvoiceItem()
                            {
                                ProductId = product.Id,
                                ProductName = product.ProductName,
                                PriceSoldToCustomer = product.SellingPrice,
                                Quantity = item.Quantity,
                                ShippingPrice = double.Parse(cart.ShippingAreaPrice ?? "0"),
                                StockQuantityFromProduct = product.StockQuantity,
                                DifferencePercentageFromProduct = product.DifferencePercentage,
                                BuyingPriceFromProduct = product.BuyingPrice,
                                MaximumDiscountPercentageFromProduct = product.MaximumDiscountPercentage,
                                SellingPriceFromProduct = product.SellingPrice,
                                OtherShopsPriceFromProduct = product.OtherShopsPrice,
                                ProductExpiryDateFromProduct = product.ProductExpiryDate,
                                ProductTagsFromProduct = product.ProductTags,
                                BarcodeFromProduct = product.Barcode,
                            };
                            grandTotalPrice += (int)(item.Quantity * product?.SellingPrice);
                            onlineOrder.InvoiceItems?.Add(invoiceItem);
                        }
                    }
                    onlineOrder.GrandTotalAmount = grandTotalPrice + onlineOrder.ShippingPrice;
                    _unitOfWork.OnlineOrder.Add(onlineOrder);
                    await _unitOfWork.Save();
                }
                return Result<string>.Success("success", "Online Order Created Successfully");

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
        public InvoiceVM CreateInvoiceForViewing(string orderNum)
        {
            try
            {
                var onlineOrder = _unitOfWork.OnlineOrder.Get(s => s.OrderNumber == orderNum, "InvoiceItems,Customer");

                var productVM = new InvoiceVM();
                var freights = _unitOfWork.ShippingFreight.GetAll().ToList();
                productVM.ListOfAreas = freights.Select(v => new SelectListItem
                {
                    Text = v.ShippingArea,
                    Value = v.Price.ToString()
                }).ToList();
                productVM.ListInvoiceItemVMs = onlineOrder.InvoiceItems?.Select(s => new InvoiceItemVM()
                {
                    ProductName = s.ProductName,
                    Quantity = s.Quantity,
                    PriceSoldToCustomer = s.PriceSoldToCustomer,
                    ShippingPrice = s.ShippingPrice,
                    CustomerName = onlineOrder.Customer?.CustomerName,
                    MobileNumber = onlineOrder.Customer?.Phone,
                }).ToList();
                return productVM;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex, ex.InnerException.Message);
                }
                else
                    _logger.LogError(ex, ex.Message);
                return new InvoiceVM();
            }

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
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex, ex.InnerException.Message);
                }
                else
                    _logger.LogError(ex, ex.Message);
                return null;
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
                        Id = product.Id,
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
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex, ex.InnerException.Message);
                }
                else
                    _logger.LogError(ex, ex.Message);
            }
            return new ProductVM();
        }
        public Result<List<OnlineOrderVM>> GetAllOrdersToBeInvoiced()
        {
            try
            {
                var orders = _unitOfWork.OnlineOrder.GetAll(s => s.IsDeleted == false, "Customer,InvoiceItems");
                var orderVMs = orders.Select(s => new OnlineOrderVM()
                {
                    Id = s.Id,
                    OrderNumber = s.OrderNumber,
                    CustomerName = s.Customer?.CustomerName,
                    OrderDate = s.OrderDate.ToString("yyyy-MM-dd:HH:mm:ss"),
                    OrderStatus = s.OrderStatus.ToString(),
                    GrandTotalAmount = s.GrandTotalAmount,
                    Status = s.OrderStatus,
                    Area = _unitOfWork.ShippingFreight.Get(d => d.Id == s.AreaId).ShippingArea,
                }).ToList();
                return Result<List<OnlineOrderVM>>.Success(orderVMs, "success");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex, ex.InnerException.Message);
                }
                else
                    _logger.LogError(ex, ex.Message);

                return Result<List<OnlineOrderVM>>.Failure(ex.InnerException.ToString(), "error");
            }
        }
        public Result<InvoiceVM> GetInvoiceForSpecificOnlineOrder(int id)
        {
            try
            {
                var onlineOrder = _unitOfWork.OnlineOrder.Get(s => s.Id == id, "InvoiceItem");
                if (onlineOrder != null)
                {
                    return Result<InvoiceVM>.Success(new InvoiceVM() { OnlineOrderId = id }, "success");
                }
                return Result<InvoiceVM>.Failure("error");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex, ex.InnerException.Message);
                }
                else
                    _logger.LogError(ex, ex.Message);
                return Result<InvoiceVM>.Failure("error");

            }
        }
        public async Task<Result<bool>> UpdateOrderStatus(string orderNum, string options)
        {
            try
            {
                var getOrder = _unitOfWork.OnlineOrder.Get(s => s.OrderNumber == orderNum);
                if (getOrder == null)
                {
                    return Result<bool>.Failure("Order not found", "error");
                }

                getOrder.OrderStatus = options switch
                {
                    "Delivered" => Status.Completed,
                    "Returned" => Status.Returned,
                    _ => Status.InProgress
                };

                _unitOfWork.OnlineOrder.Update(getOrder);
                await _unitOfWork.Save();

                return Result<bool>.Success(true, "success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status for OrderNum: {OrderNum}", orderNum);
                return Result<bool>.Failure("Error updating order status", "error");
            }
        }

    }
}
