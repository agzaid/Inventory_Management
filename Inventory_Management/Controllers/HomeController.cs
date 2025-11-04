using Application.Common.Interfaces;
using Application.Services.Implementation;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Models;
using Domain.Models;
using Infrastructure.Data;
using Inventory_Management.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Razor.Tokenizer.Symbols;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Inventory_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBrandService _brandService;
        private readonly IOnlineOrderService _onlineOrderService;
        private readonly IFeedbackService _feedbackService;
        private readonly IEmailSender _emailSender;
        private readonly IStringLocalizer _localizer;
        private readonly IMemoryCache _cache;

        public HomeController(ILogger<HomeController> logger, IBrandService brandService, IOnlineOrderService onlineOrderService, IFeedbackService feedbackService, IEmailSender emailSender, IStringLocalizer localizer, IMemoryCache cache)
        {
            _logger = logger;
            _brandService = brandService;
            _onlineOrderService = onlineOrderService;
            _feedbackService = feedbackService;
            _emailSender = emailSender;
            _localizer = localizer;
            _cache = cache;
        }
        [RateLimit(100, 60)]
        public async Task<IActionResult> Index(string? status, string? message)
        {
            var culture = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var cacheKey = $"PortalProducts_{culture}";
            // try to get from cache
            if (!_cache.TryGetValue(cacheKey, out var portal))
            {
                portal = await _onlineOrderService.GetAllProductsForPortal();

                // set cache with expiration
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(2)) // hard expiry
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1)); // reset if accessed

                _cache.Set(cacheKey, portal, cacheOptions);
            }

            if (status == "success")
                TempData["success"] = message;
            else
                TempData["error"] = message;

            return View(portal);
        }
        [RateLimit(100, 60)]
        public async Task<IActionResult> Shop()
        {
            var culture = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var cacheKey = $"ShopProducts_{culture}";
            if (!_cache.TryGetValue(cacheKey, out var products))
            {
                products = await _onlineOrderService.GetAllProductsForPortal();

                _cache.Set(cacheKey, products,
                    new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(2)));
            }

            return View(products);
        }

        [HttpGet]
        [RateLimit(100, 60)]
        public async Task<IActionResult> GetProductsByCategory(int? categoryId)
        {
            var products = await _onlineOrderService.GetProductsByCategory(categoryId);
            return PartialView("_ProductListPartial", products.Data);
        }
        [HttpGet]
        [RateLimit(100, 60)]
        public async Task<IActionResult> GetProductsByCategoryAndBrand(int categoryId, int brandId)
        {
            var products = await _onlineOrderService.GetProductsByCategoryAndBrand(categoryId, brandId);
            return PartialView("_ProductListPartial", products.Data);
        }

        [HttpGet]
        [RateLimit(100, 60)]
        public async Task<IActionResult> GetBrandsByCategory(int? categoryId)
        {
            var brands = await _brandService.GetBrandsByCategory(categoryId);
            var products = await _onlineOrderService.GetProductsByCategory(categoryId);
            return PartialView("_ProductListPartial", products.Data);
        }
        [HttpGet]
        public async Task<IActionResult> GetProductsByName(string? name)
        {
            var products = await _onlineOrderService.GetProductsByName(name);
            return PartialView("_ProductListPartial", products.Data);
        }
        [RateLimit(100, 60)]
        public IActionResult ProductDetails(int Id)
        {
            var products = _onlineOrderService.GetProductDetails(Id);

            return View(products);
        }

        [RateLimit(10, 60)]
        public async Task<IActionResult> Cart()
        {
            var cartvm = new Inventory_Management.Models.CartVM()
            {
                Areas = await _onlineOrderService.DistrictSelectList(),
                DeliverySlotVMs = await _onlineOrderService.DeliverySlot(),
            };
            return View(cartvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RateLimit(10, 60)]
        public async Task<IActionResult> CheckoutDetails([FromBody] Inventory_Management.Models.CartVM data)
        {
            if (data == null)
            {
                return BadRequest("Invalid data received.");
            }
            var mappedData = new Domain.Models.CartVM
            {
                ShippingAreaPrice = data.ShippingAreaPrice,
                OrderNumber = data.OrderNumber,
                CustomerName = data.CustomerName,
                CustomerAddress = data.CustomerAddress,
                CustomerPhone = data.CustomerPhone,
                OptionalCustomerPhone = data.OptionalCustomerPhone,
                StreetName = data.StreetName,
                BuildingNumber = data.BuildingNumber,
                Floor = data.Floor,
                ApartmentNumber = data.ApartmentNumber,
                LandMark = data.LandMark,
                SelectedSlots = data.SelectedSlots,
                Location = data.locationInput,
                AreaId = data.AreaId,
                ItemsVMs = data.ItemsVMs.Select(i => new Domain.Models.ItemsVM
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    ProductPrice = i.ProductPrice,
                    Quantity = i.Quantity
                }).ToList(),
            };
            var cart = await _onlineOrderService.CreateOrder(mappedData);
            if (cart != null)
            {
                //send an email
                var emailBody = _emailSender.HtmlTemplateForOnlineOrder(mappedData.OrderNumber, mappedData.CustomerName, mappedData.CustomerAddress, mappedData.CustomerPhone);

                await _emailSender.SendEmailAsync(
                    "ahmedzaidtp34@gmail.com",
                    "🛒 New Online Order Placed",
                    emailBody
                );

                return Json(cart);
            }
            else
                return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RateLimit(10, 60)]
        public async Task<IActionResult> Feedback(string name, string email, string subject, string phone, string message, List<IFormFile> ImagesFormFiles)
        {
            //if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message) || string.IsNullOrEmpty(phone))
            //{
            //    TempData["error"] = "Please fill empty fields";
            //    return RedirectToAction("Contact");
            //}
            var result = await _feedbackService.CreateFeedback(new FeedbackVM() { Name = name, Email = email, Subject = subject, Phone = phone, Message = message, ImagesFormFiles = ImagesFormFiles });
            if (result.Data == "success")
            {
                var emailBody = _emailSender.HtmlTemplateForOnlineOrder(null, name, null, phone);

                await _emailSender.SendEmailAsync(
                    "ahmedzaidtp34@gmail.com",
                    "Feedback from: "+ email+ " _ Subject :" + subject,
                    message
                );
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Failed to Create Feedback";

                return RedirectToAction("Index", new { status = "error", message = "Failed to Create Feedback" });
            }
        }
        //public async Task<IActionResult> GetPaginatedProducts(int pageNumber = 1, int pageSize = 20)
        //{
        //    var product = await _onlineOrderService.GetProductsPaginated(pageNumber, pageSize);
        //    return PartialView("_ProductListPartial", product.Data.Items);
        //}
        //public async Task<IActionResult> GetPaginatedProducts(int pageNumber, int pageSize, int? categoryId)
        //{
        //    var product = await _onlineOrderService.GetProductsPaginated(pageNumber, pageSize, categoryId);


        //    if (!product.Data.Items.Any())
        //    {
        //        return Content(""); // return empty string
        //    }

        //    return PartialView("_ProductListPartial", product.Data.Items);
        //}
        [HttpGet]
        public async Task<IActionResult> GetPaginatedProducts(int pageNumber, int pageSize, int? categoryId, int? brandId)
        {
            // Build unique cache key
            var cacheKey = $"Products_Page_{pageNumber}_Size_{pageSize}_Cat_{categoryId ?? 0}_Brand_{brandId ?? 0}";

            if (!_cache.TryGetValue(cacheKey, out PaginatedResult<ProductVM> product))
            {
                var result = await _onlineOrderService.GetProductsPaginated(pageNumber, pageSize, categoryId, brandId);

                // Extract the PaginatedResult<ProductVM> from the Result wrapper
                product = result?.Data;

                // Only cache if data exists
                if (product?.Items?.Any() == true)
                {
                    _cache.Set(
                        cacheKey,
                        product,
                        new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromMinutes(2)) // short cache to avoid stale
                            .SetSlidingExpiration(TimeSpan.FromMinutes(1))
                    );
                }
            }

            // If no products, return empty (do not cache empty results for too long)
            if (product == null || !product.Items.Any())
                return Content("");

            return PartialView("_ProductListPartial", product.Items);
        }

        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("Error")]
        [Route("Error/{statusCode}")]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                // Handle HTTP status code errors
                if (statusCode == 404)
                    ViewData["ErrorMessage"] = "The page you are looking for could not be found.";
                else if (statusCode == 500)
                    ViewData["ErrorMessage"] = "Internal server error occurred.";
                else
                    ViewData["ErrorMessage"] = $"An error occurred. Status code: {statusCode}";
            }
            else
            {
                // Handle unhandled exceptions
                var exception = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
                ViewData["ErrorMessage"] = "An unexpected error occurred.";
                ViewData["ExceptionPath"] = exception?.Path;
                ViewData["ExceptionMessage"] = exception?.Error.Message;
            }

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
