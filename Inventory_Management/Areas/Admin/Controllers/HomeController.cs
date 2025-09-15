using System.Diagnostics;
using Application.Services.Intrerfaces;
using Infrastructure.Data;
using Inventory_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IInvoiceService _invoiceService;
        private readonly IOnlineOrderService _onlineOrder;

        public HomeController(ILogger<HomeController> logger, IInvoiceService invoiceService,IOnlineOrderService onlineOrder)
        {
            _logger = logger;
            _invoiceService = invoiceService;
            _onlineOrder = onlineOrder;
        }

        public IActionResult Index()
        {
            var pendingOnlineOrders = _onlineOrder.GetAllOrdersPending();
            var invoices = _invoiceService.GetAllInvoices();
            var totalAmount = invoices.Sum(x => x.TotalAmount ?? 0);
            ViewBag.TotalAmount = totalAmount;
            ViewBag.PendingOnlineOrders = pendingOnlineOrders.Data.Count;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
