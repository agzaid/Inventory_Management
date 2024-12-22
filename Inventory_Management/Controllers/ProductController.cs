using Application.Services.Implementation;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public IActionResult Index()
        {
            var products = _productService.GetAllProducts();
            return View();
        }
        public IActionResult Create()
        {
            return View(_productService.CreateProductGetRequset());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM obj)
        {
            //if (ModelState.IsValid)
            //{
                var result = _productService.CreateProduct(obj);
                TempData["success"] = result;
                return RedirectToAction(nameof(Index));
            //}
            return View();
        }

        public IActionResult Edit(int id)
        {
            return View(_productService.GetProductById(id));
        }

        [HttpPost]
        public IActionResult Edit(ProductVM obj)
        {
            if (ModelState.IsValid)
            {

                var result = _productService.UpdateProduct(obj);
                if (result == true)
                {
                    TempData["success"] = "Category Updated Successfully";
                }
                else
                    TempData["error"] = result;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public IActionResult Delete(int id)
        {
            var result = _productService.DeleteProduct(id);
            if (result == true)
            {
                TempData["success"] = "Category Deleted Successfully";
            }
            else
                TempData["error"] = result;
            return RedirectToAction(nameof(Index));
        }
    }
}
