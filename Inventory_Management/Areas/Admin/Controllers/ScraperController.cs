using Application.Services.Intrerfaces;
using Infrastructure.PriceScraper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inventory_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ScraperController : Controller
    {
        private readonly PriceScraperService _priceScraperService;
        private readonly IProductService _productService;

        private readonly ILogger<ScraperController> _logger;

        public ScraperController(PriceScraperService priceScraperService, ILogger<ScraperController> logger, IProductService productService)
        {
            _priceScraperService = priceScraperService;
            _logger = logger;
            _productService = productService;
        }

        // GET: Admin/Scraper/Index - Show scraper management page
        public async Task<IActionResult> Index()
        {
            // Fetch products from your database/service
            var products = await _productService.GetAllProducts();
            return View(products);
        }

        // POST: Admin/Scraper/ScrapePriceForUrl
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ScrapePriceForUrl(string sourceUrl)
        {
            if (string.IsNullOrWhiteSpace(sourceUrl))
            {
                TempData["error"] = "Source URL is required";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _logger.LogInformation("Starting price scrape for URL: {Url}", sourceUrl);
                
                // Call the actual ScrapeAsync method from PriceScraperService
                var result = await _priceScraperService.ScrapeAsync(sourceUrl, true);

                if (result.Success)
                {
                    TempData["success"] = $"✅ Price scraped successfully!\n" +
                        $"Price: {result.Price} {result.Currency}\n" +
                        $"Method: {result.Method}\n" +
                        $"Time: {result.Timestamp:yyyy-MM-dd HH:mm:ss}";
                    
                    _logger.LogInformation("Scrape successful for URL: {Url}, Price: {Price}, Method: {Method}", 
                        sourceUrl, result.Price, result.Method);
                }
                else
                {
                    TempData["error"] = $"❌ Scraping failed: {result.Error}";
                    _logger.LogWarning("Scrape failed for URL: {Url}, Error: {Error}", sourceUrl, result.Error);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scraping price for URL: {Url}", sourceUrl);
                TempData["error"] = $"Error scraping price: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Admin/Scraper/TestScrape - For testing
        [HttpGet]
        public async Task<IActionResult> TestScrape(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return BadRequest("URL is required");
            }

            try
            {
                var result = await _priceScraperService.ScrapeAsync(url);

                return Json(new
                {
                    success = result.Success,
                    price = result.Price,
                    currency = result.Currency,
                    method = result.Method,
                    error = result.Error,
                    timestamp = result.Timestamp
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TestScrape for URL: {Url}", url);
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
