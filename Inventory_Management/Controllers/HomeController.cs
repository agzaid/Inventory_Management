using System.Diagnostics;
using Application.Services.Intrerfaces;
using Domain.Models;
using Infrastructure.Data;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Inventory_Management.Models;
using System.Web.Razor.Tokenizer.Symbols;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Application.Services.Implementation;

namespace Inventory_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOnlineOrderService _onlineOrderService;


        public HomeController(ILogger<HomeController> logger, IOnlineOrderService onlineOrderService)
        {
            _logger = logger;
            _onlineOrderService = onlineOrderService;
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
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await _onlineOrderService.GetProductsByCategory(categoryId);
            return PartialView("_ProductListPartial", products.Data);
        }
        public IActionResult ProductDetails(int Id)
        {
            var products = _onlineOrderService.GetProductDetails(Id);
            
            return View(products);
        }
       
        public async Task<IActionResult> Cart()
        {
            var cartvm = new CartVM()
            {
                Areas = await _onlineOrderService.ShippingFreightSelectList(),
                DeliverySlotVMs = await _onlineOrderService.DeliverySlot(),
            };
            return View(cartvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckoutDetails([FromBody]CartVM data)
        {
            var cart = _onlineOrderService.CreateOrder(data);
            if (cart != null)
            {
                return Json(cart.Result);
            }else
                return View(cart); 
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
