using Application.Services.Implementation;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Models;
using Domain.Models;
using Infrastructure.Data;
using Inventory_Management.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Razor.Tokenizer.Symbols;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Inventory_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOnlineOrderService _onlineOrderService;
        private readonly IStringLocalizer _localizer;

        public HomeController(ILogger<HomeController> logger, IOnlineOrderService onlineOrderService, IStringLocalizer localizer)
        {
            _logger = logger;
            _onlineOrderService = onlineOrderService;
            _localizer = localizer;
        }

        public IActionResult Index(string? message)
        {
            var portal = _onlineOrderService.GetAllProductsForPortal();
            //if (status == "success")
            //{
            //    TempData["success"] = message;
            //}
            //else
            //{
            //    TempData["error"] = message;
            //}
            //ViewData["Greeting"] = _localizer["Greeting"];
            return View(portal);
        }
        public IActionResult Shop()
        {
            var products = _onlineOrderService.GetAllProductsForPortal();
            //if (status == "success")
            //{
            //    TempData["success"] = message;
            //}
            //else
            //{
            //    TempData["error"] = message;
            //}
            return View(products);
        }
        [HttpGet]
        public async Task<IActionResult> GetProductsByCategory(int? categoryId)
        {
            var products = await _onlineOrderService.GetProductsByCategory(categoryId);
            return PartialView("_ProductListPartial", products.Data);
        }
        [HttpGet]
        public async Task<IActionResult> GetProductsByName(string? name)
        {
            var products = await _onlineOrderService.GetProductsByName(name);
            return PartialView("_ProductListPartial", products.Data);
        }
        public IActionResult ProductDetails(int Id)
        {
            var products = _onlineOrderService.GetProductDetails(Id);
            
            return View(products);
        }
       
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
        public IActionResult CheckoutDetails([FromBody] Inventory_Management.Models.CartVM data)
        {
            if (data == null)
            {
                return BadRequest("Invalid data received.");
            }
            var mappedData = new Domain.Models.CartVM
            {
                OrderNumber = data.OrderNumber,
                CustomerName = data.CustomerName,
                CustomerAddress = data.CustomerAddress,
                CustomerPhone = data.CustomerPhone,
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
            var cart = _onlineOrderService.CreateOrder(mappedData);
            if (cart != null)
            {
                return Json(cart.Result);
            }else
                return View(cart); 
        }
        public async Task<IActionResult> GetPaginatedProducts(int pageNumber = 1, int pageSize = 10)
        {
            var product = await _onlineOrderService.GetProductsPaginated(pageNumber, pageSize);
            return PartialView("_ProductListPartial", product.Data.Items);
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
            ViewBag.ErrorMessage = exception?.Message;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
