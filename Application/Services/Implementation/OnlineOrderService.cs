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
using System.Threading.Tasks;
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

        public async Task<PortalVM> GetAllProductsForPortal()
        {
            var retrievedImages = new List<string>();
            var image64 = new List<string>();
            var productVMs = new List<ProductVM>();
            var portalVM = new PortalVM();
            try
            {
                var brands = await _unitOfWork.Brand.GetAllAsync(s => s.IsDeleted == false, "Images");
                var products = _unitOfWork.Product.GetAll(s => s.IsDeleted == false, "Category,Images").OrderByDescending(s => s.Create_Date).Take(20);
                var categories = _unitOfWork.Category.GetAll(s => s.IsDeleted == false, "BrandsCategories,BrandsCategories.Brand").ToList();

                var prvm = products.Select(s => new ProductVM()
                {
                    Id = s.Id,
                    //DisplayProductName = s.DisplayProductName,
                    ProductName = s.ProductName,
                    ProductNameAr = s.ProductNameAr,
                    Description = s.Description,
                    CategoryName = s.Category?.DisplayCategoryName,
                    SellingPrice = s.SellingPrice,
                    IsKilogram = s.IsKilogram,
                    PricePerGram = s.PricePerGram,
                    OtherShopsPrice = s.OtherShopsPrice,
                    DifferencePercentage = Math.Ceiling(s.DifferencePercentage ?? 0).ToString("0.00") ?? "0.00",
                    StockQuantity = s.StockQuantity,
                    ExpiryDate = s.ProductExpiryDate?.ToString("yyyy-MM-dd"),
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                    Barcode = s.Barcode,
                    ListOfRetrievedImages = s.Images?.Select(s => s.FilePath)?.ToList(),
                }).ToList();
                portalVM.ProductVMs = prvm;
                portalVM.CategoryVMs = categories.Select(s => new CategoryVM
                {
                    Id = s.Id,
                    CategoryName = s.CategoryName?.ToUpper(),
                    CategoryNameAr = s.CategoryNameAr,
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                    Description = s.Description,
                    BrandVMs = s.BrandsCategories?.Select(bc => new BrandVM
                    {
                        Id = (int)(bc.BrandId ?? 0),
                        BrandName = bc.Brand?.BrandName,
                        BrandNameAr = bc.Brand?.BrandNameAr,
                        Description = bc.Brand?.Description,
                        CreatedDate = bc.Brand?.Create_Date?.ToString("yyyy-MM-dd"),
                        ListOfRetrievedImages = bc.Brand?.Images?.Select(a => a.FilePath).ToList(),
                    }).ToList(),
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

                var products = _unitOfWork.Product.GetAll(s => s.IsDeleted == false && (categoryId == null || s.CategoryId == categoryId), "Category,Images").OrderByDescending(a => a.Create_Date).Take(20);
                var categories = _unitOfWork.Category.GetAll(s => s.IsDeleted == false).ToList();

                var productsVMS = products.Select(s => new ProductVM()
                {
                    Id = s.Id,
                    ProductName = s.ProductName?.ToUpper(),
                    ProductNameAr = s.ProductNameAr,
                    Description = s.Description,
                    CategoryName = s.Category?.CategoryName?.ToUpper(),
                    SellingPrice = s.SellingPrice,
                    OtherShopsPrice = s.OtherShopsPrice,
                    DifferencePercentage = Math.Ceiling(s.DifferencePercentage ?? 0).ToString("0.00") ?? "0.00",
                    StockQuantity = s.StockQuantity,
                    ExpiryDate = s.ProductExpiryDate?.ToString("yyyy-MM-dd"),
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                    Barcode = s.Barcode,
                    ListOfRetrievedImages = s.Images?.Select(a => a.FilePath).ToList(),
                }).ToList();
                return Task.FromResult(Result<List<ProductVM>>.Success(productsVMS, "success"));
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
        public Task<Result<List<ProductVM>>> GetProductsByCategoryAndBrand(int? categoryId, int? brandId)
        {
            try
            {
                var products = _unitOfWork.Product.GetAll(
                    s => s.IsDeleted == false
                         && (categoryId == null || s.CategoryId == categoryId)
                         && (brandId == null || s.BrandId == brandId),
                    "Category,Images,Brand").OrderByDescending(a => a.Create_Date).Take(20);

                var productsVMS = products.Select(s => new ProductVM()
                {
                    Id = s.Id,
                    ProductName = s.ProductName?.ToUpper(),
                    ProductNameAr = s.ProductNameAr,
                    Description = s.Description,
                    CategoryName = s.Category?.CategoryName?.ToUpper(),
                    SellingPrice = s.SellingPrice,
                    OtherShopsPrice = s.OtherShopsPrice,
                    DifferencePercentage = Math.Ceiling(s.DifferencePercentage ?? 0).ToString("0.00"),
                    StockQuantity = s.StockQuantity,
                    ExpiryDate = s.ProductExpiryDate?.ToString("yyyy-MM-dd"),
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                    Barcode = s.Barcode,
                    ListOfRetrievedImages = s.Images?.Select(a => a.FilePath).ToList(),
                }).ToList();

                return Task.FromResult(Result<List<ProductVM>>.Success(productsVMS, "success"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.InnerException?.Message ?? ex.Message);
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
                //        Description = item.Description,
                //        CategoryName = item.Category?.CategoryName?.ToUpper(),
                //        SellingPrice = item.SellingPrice,
                //        OtherShopsPrice = item.OtherShopsPrice,
                //        DifferencePercentage = Math.Ceiling(item.DifferencePercentage ?? 0).ToString("0.00") ?? "0.00",
                //        StockQuantity = item.StockQuantity,
                //        ExpiryDate = item.ProductExpiryDate?.ToString("yyyy-MM-dd"),
                //        CreatedDate = item.Create_Date?.ToString("yyyy-MM-dd"),
                //        Barcode = item.Barcode,
                //        ListOfRetrievedImages = image64,
                //    };
                //    productVMs.Add(productVM);
                //}
                var productsVMS = products.Select(s => new ProductVM()
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
                    ListOfRetrievedImages = s.Images?.Select(a => a.FilePath).ToList(),
                }).ToList();

                return Task.FromResult(Result<List<ProductVM>>.Success(productsVMS, "success"));
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
        public async Task<Result<PaginatedResult<ProductVM>>> GetProductsPaginated(int pageNumber, int pageSize, int? categoryId, int? brandId)
        {
            try
            {
                Expression<Func<Product, bool>> filter = s => s.IsDeleted == false;
                // apply category filter if provided
                if (categoryId.HasValue)
                {
                    int catId = categoryId.Value; // capture for closure
                    filter = s => !s.IsDeleted && s.CategoryId == catId;
                }
                // Add brand filter if provided (combine with category if both exist)
                if (brandId.HasValue)
                {
                    int brId = brandId.Value;
                    if (categoryId.HasValue)
                        filter = s => !s.IsDeleted && s.CategoryId == categoryId && s.BrandId == brId;
                    else
                        filter = s => !s.IsDeleted && s.BrandId == brId;
                }
                //Expression<Func<Product, object>> includes = x => x.Images;
                Expression<Func<Product, object>>[] includes = { x => x.Images, x => x.Category };
                Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy;
                orderBy = s => s.OrderByDescending(s => s.ProductName);

                var products = await _unitOfWork.Product.GetPaginatedAsync(pageNumber, pageSize, orderBy, filter, includes);

                var showProducts = products.Items.Select(s => new ProductVM()
                {
                    Id = s.Id,
                    ProductName = s.ProductName.ToUpper(),
                    ProductNameAr = s.ProductNameAr,
                    Description = s.Description,
                    CategoryName = s.Category?.DisplayCategoryName,
                    SellingPrice = s.SellingPrice,
                    OtherShopsPrice = s.OtherShopsPrice,
                    DifferencePercentage = Math.Ceiling(s.DifferencePercentage ?? 0).ToString("0.00") ?? "0.00",
                    StockQuantity = s.StockQuantity,
                    ExpiryDate = s.ProductExpiryDate?.ToString("yyyy-MM-dd"),
                    CreatedDate = s.Create_Date?.ToString("yyyy-MM-dd"),
                    Barcode = s.Barcode,
                    ListOfRetrievedImages = s.Images?.Select(a => a.FilePath).ToList(),
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
        public async Task<List<SelectListItem>> DistrictSelectList()
        {
            try
            {
                var shipping = _unitOfWork.District.GetAll(s => s.IsDeleted == false, "ShippingFreight");
                var vm = shipping.Select(s => new SelectListItem()
                {
                    Text = $"{s.Name} ({s.ShippingFreight?.ShippingArea}) ({s.Price})",
                    Value = s.Id.ToString()
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
                    var existingOrder = await _unitOfWork.OnlineOrder.GetFirstOrDefaultAsync(s => s.OrderNumber == cart.OrderNumber);
                    if (existingOrder != null)
                    {
                        return Result<string>.Failure("Order with this number already exists", "error");
                    }

                    ShippingFreight shipping = null;
                    IEnumerable<DeliverySlot> deliverySlot;
                    decimal shippingPrice = 0;
                    var grandTotalPrice = 0;
                    if (cart.ShippingAreaPrice.HasValue)
                    {
                        shippingPrice = cart.ShippingAreaPrice.Value;
                        shipping = await _unitOfWork.ShippingFreight.GetFirstOrDefaultAsync(s => s.Price == shippingPrice);
                    }
                    var customer = await _unitOfWork.Customer.GetFirstOrDefaultAsync(s => s.Phone == cart.CustomerPhone);
                    if (customer == null)
                    {
                        var newCustomer = new Customer
                        {
                            CustomerName = cart.CustomerName,
                            Address = cart.CustomerAddress,
                            Phone = cart.CustomerPhone,
                            OtherPhone = cart.OptionalCustomerPhone,
                        };
                        await _unitOfWork.Customer.AddAsync(newCustomer);
                        customer = newCustomer;
                    }
                    else
                    {
                        customer.CustomerName = cart.CustomerName;
                        customer.Address = cart.CustomerAddress;
                        customer.Phone = cart.CustomerPhone;
                        customer.OtherPhone = cart.OptionalCustomerPhone;

                        _unitOfWork.Customer.Update(customer);
                    }
                    var userDeliverySlot = new List<UserDeliverySlot>();
                    if (cart.SelectedSlots?.Length > 0)
                    {
                        var deliverySlotVM = cart.SelectedSlots.Select(s => s.Split('-')[0].Trim());
                        deliverySlot = await _unitOfWork.DeliverySlot.GetAllAsync(s => deliverySlotVM.Contains(s.StartTime));
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
                        OrderNumber = cart.OrderNumber,
                        IndividualProductsNames = string.Join(", ", cart.ItemsVMs.Select(s => s.ProductName)),
                        IndividualProductsPrices = string.Join(", ", cart.ItemsVMs.Select(s => s.ProductPrice)),
                        IndividualProductsQuatities = string.Join(", ", cart.ItemsVMs.Select(s => s.Quantity)),
                        GrandTotalAmount = cart.TotalPrice,
                        AmountBeforeShipping = cart.PriceBeforeShipping,
                        Customer = customer,
                        ShippingPrice = cart.ShippingAreaPrice,
                        Address = cart.CustomerAddress,
                        DeliverySlotsAsString = cart.SelectedSlots,
                        //  UserDeliverySlots = userDeliverySlot,
                        OrderStatus = Status.InProgress,
                        OrderDate = DateTime.Now,
                        AreaId = shipping?.Id,
                        AllDiscountInput = 0,
                        //InvoiceId = 0
                        StreetName = cart.StreetName,
                        BuildingNumber = cart.BuildingNumber,
                        Floor = cart.Floor,
                        ApartmentNumber = cart.ApartmentNumber,
                        LandMark = cart.LandMark,
                        Location = cart.Location,
                    };


                    foreach (var item in cart.ItemsVMs)
                    {
                        var product = await _unitOfWork.Product.GetFirstOrDefaultAsync(s => s.Id == item.ProductId);
                        if (product != null)
                        {
                            var invoiceItem = new InvoiceItem()
                            {
                                ProductId = product.Id,
                                ProductName = product.ProductName,
                                PriceSoldToCustomer = product.SellingPrice,
                                Quantity = item.Quantity,
                                ShippingPrice = cart.ShippingAreaPrice,
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
                    await _unitOfWork.OnlineOrder.AddAsync(onlineOrder);
                    await _unitOfWork.SaveAsync();
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
                //var freights = _unitOfWork.ShippingFreight.GetAll().ToList();
                var freights = _unitOfWork.District.GetAll(s => s.IsDeleted == false, "ShippingFreight");
                productVM.ListOfAreas = freights.Select(v => new SelectListItem
                {
                    Text = $"{v.Id} - {v.Name} ({v.ShippingFreight?.ShippingArea}) ({v.Price})",
                    Value = v.Id.ToString()
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
                productVM.AreaId = onlineOrder.AreaId;
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
                        ProductNameAr = product.ProductNameAr ?? "",
                        Description = product.Description ?? "",
                        Barcode = product.Barcode ?? "",
                        ExpiryDate = product.ProductExpiryDate?.ToString("yyyy-MM-dd") ?? "",
                        SellingPrice = product.SellingPrice ?? 0m,
                        BuyingPrice = product.BuyingPrice ?? 0m,
                        OtherShopsPrice = product.OtherShopsPrice ?? 0m,
                        DifferencePercentage = product.DifferencePercentage?.ToString() ?? "",
                        MaximumDiscountPercentage = product.MaximumDiscountPercentage?.ToString() ?? "",
                        StockQuantity = product.StockQuantity ?? 0,
                        CategoryId = product.CategoryId.ToString() ?? "",
                        StatusId = product.StatusId?.ToString() ?? "",
                        ProductTags = product.ProductTags ?? "",
                        IsKilogram = product.IsKilogram,
                        ListOfRetrievedImages = product.Images?.Select(s => s.FilePath)?.ToList(),
                        //Images = result.Select(s => new Image()
                        //{
                        //    FilePath = s,
                        //    ImageName = s,
                        //    Create_Date = DateTime.Now,
                        //}).ToList()
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
                var orderVMs = orders.OrderByDescending(s => s.OrderDate).Select(s => new OnlineOrderVM()
                {
                    Id = s.Id,
                    OrderNumber = s.OrderNumber,
                    CustomerName = s.Customer?.CustomerName,
                    OrderDate = s.OrderDate.ToString("yyyy-MM-dd:HH:mm:ss"),
                    DeliverySlots = s.DeliverySlotsAsString != null ? string.Join(", ", s.DeliverySlotsAsString) : "",
                    //OrderStatus = s.OrderStatus.ToString(),
                    GrandTotalAmount = s.GrandTotalAmount,
                    Status = s.OrderStatus,
                    Address = s.Address,
                    DetailedAddress = $"{s.StreetName}, {s.BuildingNumber}, {s.Floor}, {s.ApartmentNumber}, {s.LandMark}",
                    Location = s.Location,
                    PhoneNumber = s.Customer?.Phone,
                    // Area = _unitOfWork.ShippingFreight.Get(d => d.Id == s.AreaId).ShippingArea,
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
        public Result<List<OnlineOrderVM>> GetAllOrdersPending()
        {
            try
            {
                var orders = _unitOfWork.OnlineOrder.GetAll(s => s.IsDeleted == false && s.OrderStatus == Status.InProgress);
                var orderVMs = orders.Select(s => new OnlineOrderVM()
                {
                    Id = s.Id,
                    OrderNumber = s.OrderNumber,
                    CustomerName = s.Customer?.CustomerName,
                    OrderDate = s.OrderDate.ToString("yyyy-MM-dd:HH:mm:ss"),
                    DeliverySlots = s.DeliverySlotsAsString != null ? string.Join(", ", s.DeliverySlotsAsString) : "",
                    //OrderStatus = s.OrderStatus.ToString(),
                    GrandTotalAmount = s.GrandTotalAmount,
                    Status = s.OrderStatus,
                    Address = s.Address,
                    DetailedAddress = $"{s.StreetName}, {s.BuildingNumber}, {s.Floor}, {s.ApartmentNumber}, {s.LandMark}",
                    Location = s.Location,
                    PhoneNumber = s.Customer?.Phone,
                    // Area = _unitOfWork.ShippingFreight.Get(d => d.Id == s.AreaId).ShippingArea,
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
                var getOrder = await _unitOfWork.OnlineOrder.GetFirstOrDefaultAsync(s => s.OrderNumber == orderNum);
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
                await _unitOfWork.SaveAsync();

                return Result<bool>.Success(true, "success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status for OrderNum: {OrderNum}", orderNum);
                return Result<bool>.Failure("Error updating order status", "error");
            }
        }

        //public async Task<Result<string>> CreateFeedback(FeedbackVM feedback)
        //{
        //    try
        //    {
        //        byte[] resultByteImage;
        //        List<byte[]> imagesToBeAdded = new List<byte[]>();

        //        var customer = await _unitOfWork.Customer.GetFirstOrDefaultAsync(s => s.Phone == feedback.Phone);

        //        if (feedback?.ImagesFormFiles?.Count() > 0)
        //        {
        //            foreach (var item in feedback.ImagesFormFiles)
        //            {
        //                resultByteImage = FileExtensions.ConvertImageToByteArray(item, 700, 90);
        //                imagesToBeAdded.Add(resultByteImage);
        //            }
        //        }

        //        var listOfImages = imagesToBeAdded.Select(s => new Domain.Entities.Image()
        //        {
        //            ImageByteArray = s ?? new byte[0],
        //            Create_Date = DateTime.Now,
        //        }).ToList();
        //        var newFeedback = new Feedback()
        //        {
        //            Name = feedback.Name,
        //            Email = feedback.Email,
        //            Subject = feedback.Subject,
        //            Phone = feedback.Phone,
        //            Message = feedback.Message,
        //            CustomerId = customer?.Id,
        //            Images = listOfImages
        //        };

        //        await _unitOfWork.Feedback.AddAsync(newFeedback);
        //        await _unitOfWork.SaveAsync();
        //        return Result<string>.Success("success", "Feedback Created Successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.InnerException != null)
        //        {
        //            _logger.LogError(ex, ex.InnerException.Message);
        //        }
        //        else
        //            _logger.LogError(ex, ex.Message);
        //        return Result<string>.Failure("Error Occured...", "error");
        //    }
        //}
    }
}
