# Price Scraper Implementation Guide

## Overview

The price scraper feature has been successfully integrated into your Inventory Management system. It allows you to scrape product prices from websites and store them in a dedicated `ScrapedPrice` table for price tracking and comparison.

## Architecture

### Components

1. **Domain Entity**: `ScrapedPrice`
   - Located in: `Domain\Entities\ScrapedPrice.cs`
   - Stores scraped price records with metadata

2. **Repository**: `IScrapedPriceRepository` & `ScrapedPriceRepository`
   - Located in: `Application\Common\Interfaces\IScrapedPriceRepository.cs`
   - Located in: `Infrastructure\Repo\ScrapedPriceRepository.cs`
   - Provides data access for scraped prices

3. **Service**: `IScrapedPriceService` & `ScrapedPriceService`
   - Located in: `Application\Services\Intrerfaces\IScrapedPriceService.cs`
   - Located in: `Application\Services\Implementation\ScrapedPriceService.cs`
   - Business logic for price scraping

4. **Scraper**: `PriceScraperService`
   - Located in: `Infrastructure\PriceScraper\PriceScraperService.cs`
   - Handles actual web scraping (placeholder implementation)

5. **Database Migration**: `AddScrapedPriceEntity`
   - Located in: `Infrastructure\Migrations\20250101000000_AddScrapedPriceEntity.cs`
   - Creates the `ScrapedPrice` table

## Database Table Structure

```sql
ScrapedPrice Table:
- Id (PK, int)
- ProductId (FK, int) - References Product table
- SourceUrl (nvarchar(500)) - URL of the website scraped
- Price (decimal) - Extracted price
- Currency (nvarchar(10)) - Currency code (default: "EGP")
- ScraperMethod (nvarchar(50)) - Method used (Puppeteer, Selenium, etc.)
- IsSuccessful (bit) - Whether scraping was successful
- ErrorMessage (nvarchar(500)) - Error details if failed
- ScrapedDateTime (datetime2) - When the scrape occurred
- RowVersion (rowversion) - For optimistic concurrency
- Create_Date (datetime2) - Record creation date
- Modified_Date (datetime2) - Record modification date
- IsDeleted (bit) - Soft delete flag
```

## How to Use

### 1. Apply Migration

Run the migration to create the `ScrapedPrice` table:

```bash
dotnet ef database update
```

### 2. Inject the Service

In your controller or page model:

```csharp
private readonly IScrapedPriceService _scrapedPriceService;

public YourController(IScrapedPriceService scrapedPriceService)
{
    _scrapedPriceService = scrapedPriceService;
}
```

### 3. Scrape a Price

Create a scraped price record:

```csharp
var scrapedPrice = await _scrapedPriceService.ScrapePriceAsync(productId, "https://example.com/product");
```

### 4. Retrieve Price History

Get price history for a product:

```csharp
var history = await _scrapedPriceService.GetPriceHistoryAsync(productId, take: 10);
```

### 5. Get Latest Price

Get the most recent successful scrape for a product:

```csharp
var latestPrice = await _scrapedPriceService.GetLatestPriceAsync(productId);
if (latestPrice?.IsSuccessful == true)
{
    Console.WriteLine($"Latest Price: {latestPrice.Price} {latestPrice.Currency}");
}
```

### 6. Get Prices by URL

Find all scrapes for a specific URL:

```csharp
var pricesByUrl = await _scrapedPriceService.GetPricesByUrlAsync("https://example.com/product");
```

## Implementing Web Scraping

Currently, the `PriceScraperService` is a placeholder. To enable actual web scraping, follow these steps:

### Option 1: Using PuppeteerSharp (Recommended for JavaScript-heavy sites)

1. Install the NuGet package:
```bash
dotnet add package PuppeteerSharp
```

2. Uncomment and implement the `ScrapeWithPuppeteerAsync` method in `PriceScraperService`

### Option 2: Using Selenium (Good for DOM scraping)

1. Install NuGet packages:
```bash
dotnet add package Selenium.WebDriver
dotnet add package Selenium.WebDriver.ChromeDriver
```

2. Uncomment and implement the `ScrapeWithSelenium` method in `PriceScraperService`

### Example Implementation with Puppeteer

```csharp
public async Task<ScrapeResult> ScrapeAsync(string url, bool trySeleniumFallback = true)
{
    var browserFetcher = new BrowserFetcher();
    await browserFetcher.DownloadAsync();
    
    await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions 
    { 
        Headless = true,
        Args = new[] { "--no-sandbox", "--disable-dev-shm-usage" }
    });

    await using var page = await browser.NewPageAsync();
    await page.GoToAsync(url, WaitUntilNavigation.Networkidle2);
    
    var priceText = await page.QuerySelectorAsync(".price-selector");
    var price = await page.EvaluateFunctionAsync<decimal>("el => parseFloat(el.textContent)", priceText);

    return new ScrapeResult
    {
        Success = true,
        Price = price,
        Currency = "EGP",
        Timestamp = DateTime.UtcNow,
        Method = "Puppeteer",
        Url = url
    };
}
```

## API Example Usage

### Create Scraped Price Record (Admin Area)

```csharp
[HttpPost]
[Area("Admin")]
public async Task<IActionResult> ScrapePriceForProduct(int productId, string sourceUrl)
{
    var scrapedPrice = await _scrapedPriceService.ScrapePriceAsync(productId, sourceUrl);
    
    if (scrapedPrice.IsSuccessful)
    {
        return Json(new { success = true, price = scrapedPrice.Price });
    }
    else
    {
        return Json(new { success = false, error = scrapedPrice.ErrorMessage });
    }
}
```

### Get Price History (Admin Dashboard)

```csharp
[HttpGet]
[Area("Admin")]
public async Task<IActionResult> GetPriceHistory(int productId)
{
    var history = await _scrapedPriceService.GetPriceHistoryAsync(productId, take: 30);
    return Json(history);
}
```

## Best Practices

1. **Rate Limiting**: Implement delays between requests to avoid overwhelming target servers
2. **User-Agent**: Set realistic user-agent headers to avoid being blocked
3. **Error Handling**: Always check `IsSuccessful` before using the price
4. **Caching**: Cache successful scrapes to reduce requests
5. **Robots.txt**: Respect website crawling policies
6. **Terms of Service**: Ensure compliance with website terms of service

## Troubleshooting

### "Price scraper not yet implemented" Error

This means the actual scraper methods haven't been implemented. Install PuppeteerSharp and/or Selenium, then uncomment the methods in `PriceScraperService`.

### No prices being captured

1. Check if the website has the expected HTML selectors
2. Verify network connectivity
3. Check error messages in the `ErrorMessage` field of `ScrapedPrice` records
4. Use browser developer tools to inspect the actual HTML structure

### Performance Issues

1. Use PuppeteerSharp for better performance with complex pages
2. Implement request caching
3. Schedule scraping during off-peak hours
4. Use connection pooling for database operations

## Future Enhancements

1. Add scheduled scraping jobs (using Hangfire or Quartz)
2. Implement price comparison reports
3. Add price alert notifications
4. Create analytics dashboard for price trends
5. Support multiple scraper backends (HttpClient, HtmlAgilityPack, AngleSharp)
6. Add proxy rotation support
7. Implement smart retry logic with exponential backoff
