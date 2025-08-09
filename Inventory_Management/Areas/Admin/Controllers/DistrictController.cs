using Application.Services.Implementation;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Models;


//using Inventory_Management.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inventory_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DistrictController : Controller
    {
        private readonly IDistrictService _districtService;
        private readonly ILogger<DistrictController> _logger;

        public DistrictController(IDistrictService districtService, ILogger<DistrictController> logger)
        {
            _districtService = districtService;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var result = _districtService.GetAllDistricts();
                return View(result.Data.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                throw;
            }
        }
        public IActionResult Create()
        {
            return View(_districtService.GetDistrictForCreateView());
        }

        [HttpPost]
        public async Task<IActionResult> Create(DistrictVM obj)
        {
            if (ModelState.IsValid)
            {

                var result = await _districtService.CreateDistrict(obj);
                if (result.ErrorCode == "error")
                {
                    TempData["error"] = result.Message;
                }
                else
                    TempData["success"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        public IActionResult Edit(int id)
        {
            return View(_districtService.GetDistricById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DistrictVM obj)
        {
            if (ModelState.IsValid)
            {

                var result = await _districtService.UpdateDistrict(obj);
                if (result == true)
                {
                    TempData["success"] = "Shipping Freight Updated Successfully";
                }
                else
                    TempData["error"] = result;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _districtService.DeleteDistrict(id);
            if (result == true)
            {
                TempData["success"] = "Shipping Freight Deleted Successfully";
            }
            else
                TempData["error"] = result;
            return RedirectToAction(nameof(Index));
        }
    }
}
