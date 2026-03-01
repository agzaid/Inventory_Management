using Application.Services.Intrerfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_Management.Areas.Admin.Controllers.Examples
{
    /// <summary>
    /// Example controller showing how to use the Price Scraper Service.
    /// This is an example implementation - adapt it to your needs.
    /// </summary>
    [Area("Admin")]
    [ApiController]
    [Route("api/admin/[controller]")]
    public class PriceScraperExampleController : ControllerBase
    {
        private readonly IScrapedPriceService _scrapedPriceService;

        public PriceScraperExampleController(IScrapedPriceService scrapedPriceService)
        {
            _scrapedPriceService = scrapedPriceService;
        }

        /// <summary>
        /// Scrape price for a product from a given URL
        /// POST: /api/admin/pricescraperexample/scrapeprice
        /// </summary>
        [HttpPost("scrapeprice")]
        public async Task<IActionResult> ScrapePriceForProduct(int productId, string sourceUrl)
        {
            if (string.IsNullOrWhiteSpace(sourceUrl))
            {
                return BadRequest(new { error = "sourceUrl is required" });
            }

            var scrapedPrice = await _scrapedPriceService.ScrapePriceAsync(productId, sourceUrl);

            return Ok(new
            {
                success = scrapedPrice.IsSuccessful,
                id = scrapedPrice.Id,
                productId = scrapedPrice.ProductId,
                price = scrapedPrice.Price,
                currency = scrapedPrice.Currency,
                method = scrapedPrice.ScraperMethod,
                scrapedDateTime = scrapedPrice.ScrapedDateTime,
                error = scrapedPrice.ErrorMessage
            });
        }

        /// <summary>
        /// Get price history for a product
        /// GET: /api/admin/pricescraperexample/history?productId=1&take=10
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetPriceHistory(int productId, int take = 10)
        {
            if (productId <= 0)
            {
                return BadRequest(new { error = "productId must be greater than 0" });
            }

            var history = await _scrapedPriceService.GetPriceHistoryAsync(productId, take);

            var result = new
            {
                productId,
                count = history.Count,
                prices = history.Select(x => new
                {
                    id = x.Id,
                    price = x.Price,
                    currency = x.Currency,
                    sourceUrl = x.SourceUrl,
                    method = x.ScraperMethod,
                    isSuccessful = x.IsSuccessful,
                    scrapedDateTime = x.ScrapedDateTime,
                    error = x.ErrorMessage
                }).ToList()
            };

            return Ok(result);
        }

        /// <summary>
        /// Get the latest successful price for a product
        /// GET: /api/admin/pricescraperexample/latest?productId=1
        /// </summary>
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestPrice(int productId)
        {
            if (productId <= 0)
            {
                return BadRequest(new { error = "productId must be greater than 0" });
            }

            var latestPrice = await _scrapedPriceService.GetLatestPriceAsync(productId);

            if (latestPrice == null)
            {
                return NotFound(new { message = "No successful price records found for this product" });
            }

            return Ok(new
            {
                id = latestPrice.Id,
                productId = latestPrice.ProductId,
                price = latestPrice.Price,
                currency = latestPrice.Currency,
                sourceUrl = latestPrice.SourceUrl,
                method = latestPrice.ScraperMethod,
                scrapedDateTime = latestPrice.ScrapedDateTime
            });
        }

        /// <summary>
        /// Get all prices scraped from a specific URL
        /// GET: /api/admin/pricescraperexample/byurl?url=https://example.com/product
        /// </summary>
        [HttpGet("byurl")]
        public async Task<IActionResult> GetPricesByUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return BadRequest(new { error = "url is required" });
            }

            var prices = await _scrapedPriceService.GetPricesByUrlAsync(url);

            var result = new
            {
                sourceUrl = url,
                count = prices.Count,
                prices = prices.Select(x => new
                {
                    id = x.Id,
                    productId = x.ProductId,
                    price = x.Price,
                    currency = x.Currency,
                    method = x.ScraperMethod,
                    isSuccessful = x.IsSuccessful,
                    scrapedDateTime = x.ScrapedDateTime,
                    error = x.ErrorMessage
                }).ToList()
            };

            return Ok(result);
        }

        /// <summary>
        /// Example: Get average price for a product from scraped data
        /// GET: /api/admin/pricescraperexample/average?productId=1&days=30
        /// </summary>
        [HttpGet("average")]
        public async Task<IActionResult> GetAveragePrice(int productId, int days = 30)
        {
            if (productId <= 0)
            {
                return BadRequest(new { error = "productId must be greater than 0" });
            }

            var history = await _scrapedPriceService.GetPriceHistoryAsync(productId, take: 1000);

            var thirtyDaysAgo = System.DateTime.UtcNow.AddDays(-days);
            var recentPrices = history
                .Where(x => x.IsSuccessful && x.ScrapedDateTime >= thirtyDaysAgo)
                .ToList();

            if (!recentPrices.Any())
            {
                return NotFound(new { message = $"No successful price records found for the last {days} days" });
            }

            var averagePrice = recentPrices.Average(x => x.Price);
            var minPrice = recentPrices.Min(x => x.Price);
            var maxPrice = recentPrices.Max(x => x.Price);

            return Ok(new
            {
                productId,
                days,
                recordCount = recentPrices.Count,
                averagePrice,
                minPrice,
                maxPrice,
                currency = "EGP"
            });
        }
    }
}
