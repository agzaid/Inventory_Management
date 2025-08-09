using Application.Services.Implementation;
using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Inventory_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class InvoiceController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(IInvoiceService invoiceService, ICategoryService categoryService, ILogger<InvoiceController> logger)
        {
            _categoryService = categoryService;
            _invoiceService = invoiceService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            try
            {
                var invoices = _invoiceService.GetAllInvoices();
                return View(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                throw;
            }
        }
        public IActionResult Create()
        {
            return View(_invoiceService.CreateInvoiceForViewing());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceVM obj)
        {
            if (ModelState.IsValid)
            {

                var result = await _invoiceService.CreateInvoice(obj);
                if (result[0] == "success")
                {
                    TempData["success"] = result[1];
                }
                else
                {
                    TempData["error"] = result[1];
                }
                return RedirectToAction(nameof(Index));
            }
            return View(_invoiceService.CreateInvoiceForViewing());
        }

        public IActionResult Edit(int id)
        {
            return View(_invoiceService.GetInvoiceById(id));
        }

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
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _invoiceService.DeleteInvoice(id);
            if (result == true)
            {
                TempData["success"] = "Invoice Deleted Successfully";
            }
            else
                TempData["error"] = result;
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> SearchProduct(string data)
        {
            var result = _invoiceService.SearchForProducts(data);
            var res = JsonConvert.SerializeObject(result);
            return Json(res);
        }

        [HttpGet]
        public async Task<IActionResult> SearchCustomer(string data)
        {
            var result = _invoiceService.SearchForCustomer(data);
            var res = JsonConvert.SerializeObject(result);
            return Json(res);
        }
    }
}
