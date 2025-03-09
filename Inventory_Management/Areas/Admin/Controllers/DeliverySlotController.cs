using Application.Services.Intrerfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management.Areas.admin.Controllers
{
    [Area("Admin")]
    public class DeliverySlotController : Controller
    {
        private readonly IDeliverySlotService _deliverySlotService;
        private readonly ILogger<DeliverySlotController> _logger;

        public DeliverySlotController(IDeliverySlotService deliveryService, ILogger<DeliverySlotController> logger)
        {
            _deliverySlotService = deliveryService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            try
            {
                var categories = _deliverySlotService.GetAllDeliverySlot();
                return View(categories.ToList());
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
        public async Task<IActionResult> Create(DeliverySlotVM obj)
        {
            if (ModelState.IsValid)
            {

                var result = await _deliverySlotService.CreateDeliverySlot(obj);
                TempData["success"] = result;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public IActionResult Edit(int id)
        {
            return View(_deliverySlotService.GetDeliveryById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DeliverySlotVM obj)
        {
            if (ModelState.IsValid)
            {

                var result = await _deliverySlotService.UpdateDeliverySlot(obj);
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
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _deliverySlotService.DeleteDeliverySlot(id);
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
