using Application.Services.Intrerfaces;
using Domain.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management.Areas.admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]

    public class ReportController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<ReportController> _logger;
        private readonly IAntiforgery _antiforgery;

        public ReportController(IInvoiceService invoiceService, ILogger<ReportController> logger, IAntiforgery antiforgery)
        {
            _invoiceService = invoiceService;
            _logger = logger;
            _antiforgery = antiforgery;
        }


        [HttpGet]
        public async Task<IActionResult> MonthlyInventoryReport(DateTime? startDate, DateTime? endDate)
        {
            var from = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var to = endDate ?? from.AddMonths(1);

            var report = await _invoiceService.GetReportByDateRangeAsync(from, to);

            var vm = new MonthlyInventoryFilterVM
            {
                StartDate = from,
                EndDate = to,
                Report = report
            };

            return View(vm);
        }


    }
}
