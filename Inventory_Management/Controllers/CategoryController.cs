using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Models;

//using Inventory_Management.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            try
            {
                var categories = _categoryService.GetAllCategories();
                TempData["success"] = TempData["success"];
                TempData["error"] = TempData["success"];
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
        public async Task<IActionResult> Create(CategoryVM obj)
        {
            if (ModelState.IsValid)
            {

                var result = await _categoryService.CreateCategory(obj);
                TempData["success"] = result;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public IActionResult Edit(int id)
        {
            return View(_categoryService.GetCategoryById(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryVM obj)
        {
            if (ModelState.IsValid)
            {

                var result = await _categoryService.UpdateCategory(obj);
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
            var result = await _categoryService.DeleteCategory(id);
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
