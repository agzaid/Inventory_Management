using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(ProductVM product)
        {
            return View();
        }
    }
}
