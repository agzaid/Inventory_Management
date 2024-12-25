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
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var products = _productService.GetAllProducts();
            if (TempData["success"] != null)
            {
                TempData["success"] = TempData["success"];
            }
            else if (TempData["error"] != null)
            {
                TempData["error"] = TempData["error"];
                TempData["error"] = TempData["error"];
                ViewBag.error = TempData["error"];
            }

            return View(products);
        }
        public IActionResult Create()
        {
            return View(_productService.CreateProductForViewing());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM obj)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService.CreateProduct(obj);
                if (result != null && result[0] == "success")
                {
                    TempData["success"] = result[1];
                }
                else
                    TempData["error"] = result[1];

                return RedirectToAction(nameof(Index));
            }
            return View(obj);
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
