using Application.Services.Intrerfaces;
using Infrastructure.PriceScraper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Entities;
using Application.Common.Interfaces;

namespace Inventory_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ScraperController : Controller
    {
        private readonly PriceScraperService _priceScraperService;
        private readonly IProductService _productService;
        private readonly IScrapedPriceService _scrapedPriceService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ScraperController> _logger;

        public ScraperController(PriceScraperService priceScraperService, ILogger<ScraperController> logger, IProductService productService, IScrapedPriceService scrapedPriceService, IUnitOfWork unitOfWork)
        {
            _priceScraperService = priceScraperService;
            _logger = logger;
            _productService = productService;
            _scrapedPriceService = scrapedPriceService;
            _unitOfWork = unitOfWork;
        }

        // GET: Admin/Scraper/Index - Show scraper management page
        public async Task<IActionResult> Index()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            // 1. Fetch all products (ONE database call)
            var products = await _productService.GetAllProducts();
            var productIds = products.Select(p => p.Id ?? 0).ToList();

            // 2. Fetch ALL latest prices for these IDs (ONE database call)
            // You will need to create this method in your service
            var latestPrices = await _scrapedPriceService.GetLatestPricesForListAsync(productIds);

            // 3. Match them up in memory (Super fast!)
            foreach (var product in products)
            {
                var priceEntry = latestPrices.FirstOrDefault(x => x.ProductId == product.Id);
                if (priceEntry != null)
                {
                    product.LastScrapedUrl = priceEntry.SourceUrl;
                    product.LastScrapedPrice = priceEntry.Price;
                    product.LastScrapedDate = priceEntry.ScrapedDateTime.ToString("yyyy-MM-dd HH:mm");
                }
            }

            watch.Stop();
            _logger.LogInformation($"PERF: Index optimized! Took {watch.ElapsedMilliseconds}ms");

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

        // GET: Admin/Scraper/ScrapeAndSave - For UI usage: scrape + save successful result
        [HttpGet]
        public async Task<IActionResult> ScrapeAndSave(int productId, string url)
        {
            if (productId <= 0)
            {
                return BadRequest("productId is required");
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                return BadRequest("URL is required");
            }

            try
            {
                var result = await _priceScraperService.ScrapeAsync(productId, url);

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
                _logger.LogError(ex, "Error in ScrapeAndSave for ProductId: {ProductId}, URL: {Url}", productId, url);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST: Admin/Scraper/ApplyPrice - Apply scraped price to product
        [HttpPost]
        public async Task<IActionResult> ApplyPrice(int productId)
        {
            try
            {
                // Get the latest successful scraped price for this product
                var latestScrape = await _scrapedPriceService.GetLatestPriceAsync(productId);
                if (latestScrape == null)
                {
                    return Json(new { success = false, error = "No scraped price found for this product" });
                }

                // Get the product
                var product = await _unitOfWork.Product.GetFirstOrDefaultAsync(p => p.Id == productId);
                if (product == null)
                {
                    return Json(new { success = false, error = "Product not found" });
                }

                // Update the product's selling price
                product.SellingPrice = latestScrape.Price;
                product.Modified_Date = DateTime.UtcNow;
                
                _unitOfWork.Product.Update(product);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Applied scraped price {Price} to product {ProductId}", latestScrape.Price, productId);

                return Json(new { 
                    success = true, 
                    newPrice = latestScrape.Price,
                    message = $"Price updated to {latestScrape.Price} EGP"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying scraped price for ProductId: {ProductId}", productId);
                return Json(new { success = false, error = ex.Message });
            }
        }

        // GET: Admin/Scraper/GetAllProductsWithUrls
        [HttpGet]
        public async Task<IActionResult> GetAllProductsWithUrls()
        {
            try
            {
                var products = await _productService.GetAllProducts();
                var productsWithUrls = products
                    .Where(p => !string.IsNullOrEmpty(p.LastScrapedUrl))
                    .Select(p => new { id = p.Id, url = p.LastScrapedUrl })
                    .ToList();
                
                return Json(productsWithUrls);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products with URLs");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: Admin/Scraper/GetAllProductsWithScrapedPrices
        [HttpGet]
        public async Task<IActionResult> GetAllProductsWithScrapedPrices()
        {
            try
            {
                var products = await _productService.GetAllProducts();
                var productIds = products.Select(p => p.Id ?? 0).ToList();
                var latestPrices = await _scrapedPriceService.GetLatestPricesForListAsync(productIds);
                
                var result = products
                    .Where(p => latestPrices.Any(lp => lp.ProductId == p.Id))
                    .Select(p => {
                        var price = latestPrices.FirstOrDefault(lp => lp.ProductId == p.Id);
                        return new {
                            id = p.Id,
                            name = p.ProductName,
                            scrapedPrice = price?.Price ?? 0
                        };
                    })
                    .ToList();
                
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching products with scraped prices");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // POST: Admin/Scraper/UpdateProductPrice
        [HttpPost]
        public async Task<IActionResult> UpdateProductPrice(int productId, decimal newPrice)
        {
            try
            {
                var product = await _unitOfWork.Product.GetFirstOrDefaultAsync(p => p.Id == productId);
                if (product == null)
                {
                    return Json(new { success = false, error = "Product not found" });
                }

                product.SellingPrice = newPrice;
                product.Modified_Date = DateTime.UtcNow;
                
                _unitOfWork.Product.Update(product);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Updated price for ProductId: {ProductId} to {Price}", productId, newPrice);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating price for ProductId: {ProductId}", productId);
                return Json(new { success = false, error = ex.Message });
            }
        }

        // GEt: Admin/Scraper/ScheduledScrapeAndUpdate
        public async Task<IActionResult> ScheduledScrapeAndUpdate()
        {
            try
            {
                _logger.LogInformation("Starting scheduled scrape and update task");
                
                var scrapedCount = 0;
                var updatedCount = 0;
                var failedCount = 0;
                const decimal decreaseAmount = 5m; // Fixed 5 EGP decrease for scheduled tasks

                // Get all products with URLs
                var products = await _productService.GetAllProducts();
                var productsWithUrls = products
                    .Where(p => !string.IsNullOrEmpty(p.LastScrapedUrl))
                    .ToList();

                // Phase 1: Scrape all products
                foreach (var product in productsWithUrls)
                {
                    try
                    {
                        var scrapeResult = await _priceScraperService.ScrapeAsync(product.Id ?? 0, product.LastScrapedUrl);
                        if (scrapeResult.Success)
                        {
                            scrapedCount++;
                            _logger.LogInformation("Successfully scraped product {ProductId}", product.Id);
                        }
                        else
                        {
                            _logger.LogWarning("Failed to scrape product {ProductId}: {Error}", product.Id, scrapeResult.Error);
                        }
                        
                        // Small delay to be respectful to target servers
                        await Task.Delay(500);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error scraping product {ProductId}", product.Id);
                        failedCount++;
                    }
                }

                // Phase 2: Update all prices with scraped prices
                var productIds = products.Select(p => p.Id ?? 0).ToList();
                var latestPrices = await _scrapedPriceService.GetLatestPricesForListAsync(productIds);
                
                foreach (var priceEntry in latestPrices)
                {
                    try
                    {
                        var product = await _unitOfWork.Product.GetFirstOrDefaultAsync(p => p.Id == priceEntry.ProductId);
                        if (product != null)
                        {
                            // Apply 5 EGP decrease
                            var newPrice = Math.Max(0, priceEntry.Price - decreaseAmount);
                            product.SellingPrice = newPrice;
                            product.Modified_Date = DateTime.UtcNow;
                            
                            _unitOfWork.Product.Update(product);
                            updatedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error updating price for product {ProductId}", priceEntry.ProductId);
                    }
                }

                // Save all price updates
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Scheduled task completed. Scraped: {Scraped}, Updated: {Updated}, Failed: {Failed}", 
                    scrapedCount, updatedCount, failedCount);

                return Json(new { 
                    success = true, 
                    scrapedCount = scrapedCount,
                    updatedCount = updatedCount,
                    failedCount = failedCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in scheduled scrape and update task");
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
