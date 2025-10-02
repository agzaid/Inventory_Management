using Application.Services.Intrerfaces;
using Domain.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management.Areas.admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SellerController : Controller
    {
        private readonly ISellerService _sellerService;
        private readonly ILogger<SellerController> _logger;
        private readonly IAntiforgery _antiforgery;

        public SellerController(ISellerService SellerService, ILogger<SellerController> logger, IAntiforgery antiforgery)
        {
            _sellerService = SellerService;
            _logger = logger;
            _antiforgery = antiforgery;
        }
        public IActionResult Index(string? status, string? message)
        {
            try
            {
                var Sellers = _sellerService.GetAllSellers();
                if (status == "success")
                {
                    TempData["success"] = message;
                }
                else
                {
                    TempData["error"] = message;
                }

                return View(Sellers.ToList());
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
        public async Task<IActionResult> Create(SellerVM obj)
        {
            if (ModelState.IsValid)
            {
                var result = await _sellerService.CreateSeller(obj);
                TempData["success"] = result.Message;
                return RedirectToAction(nameof(Index), new { status = result.Data, message = result.Message });
            }
            return View();
        }

        public IActionResult Edit(int id)
        {
            return View(_sellerService.GetSellerById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SellerVM obj)
        {
            if (ModelState.IsValid)
            {

                var result = await _sellerService.UpdateSeller(obj);
                if (result == true)
                {
                    TempData["success"] = "Seller Updated Successfully";
                    return RedirectToAction(nameof(Index), new { status = "success", message = "Seller Updated Successfully" });
                }
                else
                    return RedirectToAction(nameof(Index), new { status = "error", message = "Something Went wrong" });
            }
            return View();
        }
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _sellerService.DeleteSeller(id);
            if (result == true)
            {
                TempData["success"] = "Seller Updated Successfully";
                return RedirectToAction(nameof(Index), new { status = "success", message = "Seller Updated Successfully" });
            }
            else
                return RedirectToAction(nameof(Index), new { status = "error", message = "Something Went wrong" });
        }

        public async Task<IActionResult> GetPaginatedSeller(int pageNumber = 1, int pageSize = 2)
        {
            var Seller = await _sellerService.GetSellerPaginated(pageNumber, pageSize);
            return View(Seller);
        }
    }
}
