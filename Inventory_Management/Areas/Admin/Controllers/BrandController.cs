using Application.Services.Intrerfaces;
using Domain.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management.Areas.admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;
        private readonly ILogger<BrandController> _logger;
        private readonly IAntiforgery _antiforgery;

        public BrandController(IBrandService brandService, ILogger<BrandController> logger, IAntiforgery antiforgery)
        {
            _brandService = brandService;
            _logger = logger;
            _antiforgery = antiforgery;
        }
        public IActionResult Index(string? status, string? message)
        {
            try
            {
                var brands = _brandService.GetAllBrands();
                if (status == "success")
                {
                    TempData["success"] = message;
                }
                else
                {
                    TempData["error"] = message;
                }

                return View(brands.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                throw;
            }
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandVM obj)
        {
            var savedToken = TempData["FormToken"];
            if (obj.FormToken?.Trim() == savedToken?.ToString()?.Trim())
            {
                if (ModelState.IsValid)
                {
                    var result = await _brandService.CreateBrand(obj);
                    TempData["success"] = result;
                    TempData["FormToken"] = null;
                    return RedirectToAction(nameof(Index), new { status = "success", message = result[1] });
                }
            }
            return View();
        }

        public IActionResult Edit(int id)
        {
            return View(_brandService.GetBrandById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BrandVM obj)
        {
            if (ModelState.IsValid)
            {

                var result = await _brandService.UpdateBrand(obj);
                if (result == true)
                {
                    TempData["success"] = "Brand Updated Successfully";
                }
                else
                    TempData["error"] = result;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _brandService.DeleteBrand(id);
            if (result == true)
            {
                TempData["success"] = "Brand Deleted Successfully";
            }
            else
                TempData["error"] = result;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetPaginatedBrand(int pageNumber = 1, int pageSize = 2)
        {
            var brand = await _brandService.GetBrandPaginated(pageNumber, pageSize);
            return View(brand);
        }
    }
}
