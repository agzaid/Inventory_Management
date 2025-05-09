using Application.Services.Implementation;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management.Areas.admin.Controllers
{
    [Area("Admin")]
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
        public IActionResult Index(string? status, string? message)
        {
            var products = _productService.GetAllProducts();
            if (status == "success")
            {
                TempData["success"] = message;
            }
            else
            {
                TempData["error"] = message;
            }

            return View(products);
        }
        public IActionResult Create()
        {
            return View(_productService.CreateProductForViewingInCreate());
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
                    //TempData["success"] = result[1];
                    return RedirectToAction(nameof(Index), new { status = "success", message = result[1] });
                }
                else
                    //TempData["error"] = result[1];
                    return RedirectToAction(nameof(Index), new { status = "error", message = result[1] });
            }
            return View(_productService.CreateProductForViewingInCreate(obj));
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
                    //TempData["success"] = result[1];
                    return RedirectToAction(nameof(Index), new { status = "success", message = "Product Updated Successfully" });
                }
                else
                    TempData["error"] = "Error Occured...!!!";
                return View(obj);
            }
            return View(obj);
        }
        public IActionResult Delete(int id)
        {
            var result = _productService.DeleteProduct(id);
            if (result == true)
            {
                TempData["success"] = "Product Deleted Successfully";
            }
            else
                TempData["error"] = result;
            return RedirectToAction(nameof(Index));
        }
        public IActionResult HardDelete(int id)
        {
            var result = _productService.HardDeleteProduct(id);
            if (result == true)
            {
                TempData["success"] = "Product Deleted Successfully";
            }
            else
                TempData["error"] = result;
            return RedirectToAction(nameof(Index));
        }
    }
}
