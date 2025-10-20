using Application.Services.Implementation;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inventory_Management.Areas.admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> Index(string? status, string? message)
        {
            var products = await _productService.GetAllProducts();
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
                if (result != null && result.Data == "success")
                {
                    //TempData["success"] = result[1];
                    return RedirectToAction(nameof(Index), new { status = "success", message = result.Message });
                }
                else
                    //TempData["error"] = result[1];
                    return RedirectToAction(nameof(Index), new { status = "error", message = result.Message });
            }
            return View(obj);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _productService.GetProductById(id);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductVM obj)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService.UpdateProduct(obj);
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
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteProduct(id);
            if (result == true)
            {
                TempData["success"] = "Product Deleted Successfully";
            }
            else
                TempData["error"] = result;
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> HardDelete(int id)
        {
            var result = await _productService.HardDeleteProduct(id);
            if (result == true)
            {
                TempData["success"] = "Product Deleted Successfully";
            }
            else
                TempData["error"] = result;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateSlugs()
        {
            var count = await _productService.UpdateAllSlugsAsync();
            return Ok($"{count} product slugs updated successfully.");
        }
    }
}
