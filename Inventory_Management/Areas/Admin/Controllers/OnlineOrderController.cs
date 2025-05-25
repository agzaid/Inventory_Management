using Application.Services.Implementation;
using Application.Services.Intrerfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Management.Areas.admin.Controllers
{
    [Area("Admin")]
    public class OnlineOrderController : Controller
    {
        private readonly IOnlineOrderService _onlineOrderService;
        private readonly ILogger<OnlineOrderController> _logger;

        public OnlineOrderController(IOnlineOrderService onlineOrderService, ILogger<OnlineOrderController> logger)
        {
            _onlineOrderService = onlineOrderService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            try
            {
                var orders = _onlineOrderService.GetAllOrdersToBeInvoiced();
                if (orders.IsSuccess)
                {
                    return View(orders.Data);
                }
                else
                    return View(orders.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                throw;
            }
        }
        // [Route("admin/onlineOrder/create/{orderNum?}")]
        public IActionResult ViewOrder(string? orderNum)//remove this later 
        {
            if (!orderNum.IsNullOrEmpty())
            {
                ViewBag.orderNum = orderNum;
                return View(_onlineOrderService.CreateInvoiceForViewing(orderNum));
            }
            else
               return RedirectToAction("Index");
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(CartVM obj)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        var result = await _onlineOrderService.CreateOrder(obj);
        //        TempData["success"] = result;
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View();
        //}

        //public IActionResult Edit(int id)
        //{
        //    return View(_categoryService.GetCategoryById(id));
        //}

        //[HttpPost]
        //public async Task<IActionResult> Edit(CategoryVM obj)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        var result = await _categoryService.UpdateCategory(obj);
        //        if (result == true)
        //        {
        //            TempData["success"] = "Category Updated Successfully";
        //        }
        //        else
        //            TempData["error"] = result;
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View();
        //}
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var result = await _categoryService.DeleteCategory(id);
        //    if (result == true)
        //    {
        //        TempData["success"] = "Category Deleted Successfully";
        //    }
        //    else
        //        TempData["error"] = result;
        //    return RedirectToAction(nameof(Index));
        //}

        //public async Task<IActionResult> GetPaginatedCategory(int pageNumber = 1, int pageSize = 2)
        //{
        //    var category = await _categoryService.GetCategoryPaginated(pageNumber, pageSize);
        //    return View(category);
        //}
    }
}
