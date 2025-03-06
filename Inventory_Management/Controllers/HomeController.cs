using System.Diagnostics;
using Application.Services.Intrerfaces;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public IActionResult Index()
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
        public IActionResult ProductDetails(int Id)
        {
            var products = _onlineOrderService.GetProductDetails(Id);
            
            return View(products);
        }
       
        public IActionResult Cart()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckoutDetails([FromBody]CartVM data)
        {
            var cart = _onlineOrderService.CreateOrder(data);
            return RedirectToAction("Index","Home");
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
