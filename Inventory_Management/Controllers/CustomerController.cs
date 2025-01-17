using Application.Services.Intrerfaces;
using Domain.Entities;
using Domain.Models;

//using Inventory_Management.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            try
            {
                var customers = _customerService.GetAllCustomers();
                return View(customers.Data.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                throw;
            }
        }
        public IActionResult Create()
        {
            return View(_customerService.CreateCustomerForViewing());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CustomerVM obj)
        {
            if (ModelState.IsValid)
            {
                var result = await _customerService.CreateCustomer(obj);
                if (result.IsSuccess == true)
                {
                    TempData["success"] = result.Data;
                    return RedirectToAction(nameof(Index));
                }
            }
            var secondResult = _customerService.CreateCustomerForViewing();
            obj.ListOfAreas = secondResult.ListOfAreas;
            return View(obj);
        }

        public IActionResult Edit(int id)
        {
            return View(_customerService.GetCustomerById(id).Data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CustomerVM obj)
        {
            if (ModelState.IsValid)
            {

                var result = await _customerService.UpdateCustomer(obj);
                if (result == true)
                {
                    TempData["success"] = "Customer Updated Successfully";
                }
                else
                    TempData["error"] = result;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerService.DeleteCustomer(id);
            if (result == true)
            {
                TempData["success"] = "Customer Deleted Successfully";
            }
            else
                TempData["error"] = "error";
            return RedirectToAction(nameof(Index));
        }
    }
}
