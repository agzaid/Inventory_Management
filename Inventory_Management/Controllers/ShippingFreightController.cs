using Application.Services.Implementation;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Models;

//using Inventory_Management.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management.Controllers
{
    public class ShippingFreightController : Controller
    {
        private readonly IShippingFreightService _shippingFreightService;
        private readonly ILogger<ShippingFreightController> _logger;

        public ShippingFreightController(IShippingFreightService shippingFrieghtService, ILogger<ShippingFreightController> logger)
        {
            _shippingFreightService = shippingFrieghtService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            try
            {
                var result = _shippingFreightService.GetAllShippingFrieght();
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ShippingFreightVM obj)
        {
            if (ModelState.IsValid)
            {

                var result = await _shippingFreightService.CreateShippingFreight(obj);
                if (result.ErrorCode == "error")
                {
                    TempData["error"] = result.Message;
                }
                else
                    TempData["success"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public IActionResult Edit(int id)
        {
            return View(_shippingFreightService.GetShippingFreightById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ShippingFreightVM obj)
        {
            if (ModelState.IsValid)
            {

                var result = await _shippingFreightService.UpdateShippingFreight(obj);
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
            var result = await _shippingFreightService.DeleteShippingFreight(id);
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
