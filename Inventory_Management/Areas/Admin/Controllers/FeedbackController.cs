using Application.Services.Intrerfaces;
using Domain.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management.Areas.admin.Controllers
{
    [Area("Admin")]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ILogger<FeedbackController> _logger;
        private readonly IAntiforgery _antiforgery;

        public FeedbackController(IFeedbackService feedbackService, ILogger<FeedbackController> logger, IAntiforgery antiforgery)
        {
            _feedbackService = feedbackService;
            _logger = logger;
            _antiforgery = antiforgery;
        }
        public IActionResult Index(string? status, string? message)
        {
            try
            {
                var brands = _feedbackService.GetAllFeedbacks();
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
        public async Task<IActionResult> Create(FeedbackVM obj)
        {
            if (ModelState.IsValid)
            {
                var result = await _feedbackService.CreateFeedback(obj);
                if (result.Data == "success")
                {
                    return RedirectToAction(nameof(Index), new { status = "success", message = "Feedback Updated Successfully" });
                }
                else
                    return RedirectToAction(nameof(Index), new { status = "error", message = "Something Went wrong" });
            }
            return View();
        }

        public IActionResult Edit(int id)
        {
            return View(_feedbackService.GetFeedbackById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(FeedbackVM obj)
        {
            if (ModelState.IsValid)
            {
                var result = await _feedbackService.UpdateFeedback(obj);
                if (result == true)
                {
                    return RedirectToAction(nameof(Index), new { status = "success", message = "Feedback Updated Successfully" });
                }
                else
                    return RedirectToAction(nameof(Index), new { status = "error", message = "Something Went wrong" });
            }
            return View();
        }
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _feedbackService.DeleteFeedback(id);
            if (result == true)
            {
                return RedirectToAction(nameof(Index), new { status = "success", message = "Feedback Deleted Successfully" });
            }
            else
                return RedirectToAction(nameof(Index), new { status = "error", message = "Something Went wrong" });
        }

        public async Task<IActionResult> GetPaginatedBrand(int pageNumber = 1, int pageSize = 2)
        {
            var brand = await _feedbackService.GetFeedbackPaginated(pageNumber, pageSize);
            return View(brand);
        }
    }
}
