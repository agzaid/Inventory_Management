using Application.Services.Implementation;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.ExcelImporter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Inventory_Management.Areas.admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ExcelImporterService _excelImporter;
        private readonly ExcelImporterXML _importerXML;

        public ProductController(IProductService productService, ICategoryService categoryService, ExcelImporterService excelImporter, ExcelImporterXML importerXML)
        {
            _productService = productService;
            _categoryService = categoryService;
            _excelImporter = excelImporter;
            _importerXML = importerXML;
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

        [HttpGet]
        public  IActionResult BulkUpload()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> BulkUpload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please upload a valid file.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            var products = _excelImporter.ReadProducts(stream);
            var result = await _productService.BulkUpdateProductsAsync(products);
            if (result > 0)
            {
                TempData["success"] = $"Products updated successfully{result}";
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please upload a valid file.");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            var productss = _importerXML.ReadProducts(stream);
            var products = _excelImporter.ReadProducts(stream);
            //_productService.UpdateProduct
            //_context.Products.AddRange(products);
            //await _context.SaveChangesAsync();

            return Ok($"Uploaded {products.Count} products successfully");
        }
    }
}
