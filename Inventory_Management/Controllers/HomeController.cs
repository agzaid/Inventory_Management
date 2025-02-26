using System.Diagnostics;
using Application.Services.Intrerfaces;
using Infrastructure.Data;
using Inventory_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Inventory_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public IActionResult Index()
        {
            var products = _productService.GetAllProductsForPortal();
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
            var products = _productService.GetAllProductsForPortal();
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
            var products = _productService.GetProductDetails(Id);
            
            return View(products);
        }
       
        public IActionResult Cart()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckoutDetails([FromBody]List<CartVM> data)
        {

            return View();
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
