using Application.Services.Intrerfaces;
using Domain.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inventory_Management.Areas.admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;
        private readonly IAntiforgery _antiforgery;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger, IAntiforgery antiforgery)
        {
            _categoryService = categoryService;
            _logger = logger;
            _antiforgery = antiforgery;
        }
        public IActionResult Index()
        {
            try
            {
                var categories = _categoryService.GetAllCategories();
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
        [ValidateAntiForgeryToken]
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

        public async Task<IActionResult> Edit(int id)
        {
            return View(await _categoryService.GetCategoryById(id));
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

        public async Task<IActionResult> GetPaginatedCategory(int pageNumber = 1, int pageSize = 2)
        {
            var category = await _categoryService.GetCategoryPaginated(pageNumber, pageSize);
            return View(category);
        }
    }
}
