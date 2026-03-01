# Price Scraper Quick Reference

## Installation

```bash
# Apply database migration
dotnet ef database update

# (Optional) Install web scraping libraries
dotnet add package PuppeteerSharp
# OR
dotnet add package Selenium.WebDriver Selenium.WebDriver.ChromeDriver
```

## Basic Usage

### Inject Service
```csharp
public class ProductController : Controller
{
    private readonly IScrapedPriceService _scrapedPriceService;
    
    public ProductController(IScrapedPriceService scrapedPriceService)
    {
        _scrapedPriceService = scrapedPriceService;
    }
}
```

### Save Scraped Price
```csharp
// Create a scraped price record
var result = await _scrapedPriceService.ScrapePriceAsync(
    productId: 5, 
    sourceUrl: "https://example.com/product"
);

if (result.IsSuccessful)
{
    Console.WriteLine($"Price: {result.Price} {result.Currency}");
}
else
{
    Console.WriteLine($"Error: {result.ErrorMessage}");
}
```

### Get Price History
```csharp
// Get last 10 scrapes for a product
var history = await _scrapedPriceService.GetPriceHistoryAsync(productId: 5, take: 10);

foreach (var record in history)
{
    Console.WriteLine($"{record.ScrapedDateTime}: {record.Price} - {record.SourceUrl}");
}
```

### Get Latest Price
```csharp
// Get most recent successful scrape
var latest = await _scrapedPriceService.GetLatestPriceAsync(productId: 5);

if (latest != null)
{
    Console.WriteLine($"Latest: {latest.Price} ({latest.ScrapedDateTime})");
}
```

### Get Prices by URL
```csharp
// Find all scrapes from a specific URL
var urlPrices = await _scrapedPriceService.GetPricesByUrlAsync("https://example.com/product");

Console.WriteLine($"Found {urlPrices.Count} prices from this URL");
```

## API Endpoints (Example Controller)

### Scrape Price
```
POST /api/admin/pricescraperexample/scrapeprice
Body: { "productId": 5, "sourceUrl": "https://example.com/product" }
Response: { "success": true, "price": 99.99, "id": 1 }
```

### Get Price History
```
GET /api/admin/pricescraperexample/history?productId=5&take=10
Response: { "productId": 5, "count": 10, "prices": [...] }
```

### Get Latest Price
```
GET /api/admin/pricescraperexample/latest?productId=5
Response: { "id": 1, "price": 99.99, "scrapedDateTime": "2024-01-01..." }
```

### Get Prices by URL
```
GET /api/admin/pricescraperexample/byurl?url=https://example.com/product
Response: { "sourceUrl": "...", "count": 5, "prices": [...] }
```

### Get Average Price
```
GET /api/admin/pricescraperexample/average?productId=5&days=30
Response: { "averagePrice": 95.50, "minPrice": 90.00, "maxPrice": 100.00 }
```

## Database Queries

### Get all scraped prices for a product
```sql
SELECT * FROM ScrapedPrice 
WHERE ProductId = @ProductId AND IsDeleted = 0
ORDER BY ScrapedDateTime DESC;
```

### Get prices from specific URL
```sql
SELECT * FROM ScrapedPrice 
WHERE SourceUrl = @SourceUrl AND IsDeleted = 0
ORDER BY ScrapedDateTime DESC;
```

### Get successful scrapes in last 7 days
```sql
SELECT * FROM ScrapedPrice 
WHERE IsSuccessful = 1 
  AND ScrapedDateTime >= DATEADD(day, -7, GETUTCDATE())
ORDER BY ScrapedDateTime DESC;
```

### Calculate average price by product
```sql
SELECT 
    ProductId,
    AVG(Price) AS AvgPrice,
    MIN(Price) AS MinPrice,
    MAX(Price) AS MaxPrice,
    COUNT(*) AS RecordCount
FROM ScrapedPrice 
WHERE IsSuccessful = 1 AND IsDeleted = 0
GROUP BY ProductId;
```

## Entity Model

```csharp
public class ScrapedPrice : BaseEntity
{
    public int ProductId { get; set; }              // FK to Product
    public virtual Product Product { get; set; }
    public string SourceUrl { get; set; }           // URL scraped
    public decimal Price { get; set; }              // Extracted price
    public string Currency { get; set; }            // "EGP"
    public string ScraperMethod { get; set; }       // "Puppeteer", "Selenium", etc.
    public bool IsSuccessful { get; set; }          // Success flag
    public string ErrorMessage { get; set; }        // Error details
    public DateTime ScrapedDateTime { get; set; }   // When scraped
    public byte[] RowVersion { get; set; }          // Concurrency token
    
    // From BaseEntity
    public int Id { get; set; }
    public DateTime? Create_Date { get; set; }
    public DateTime? Modified_Date { get; set; }
    public bool IsDeleted { get; set; }
}
```

## Repository Interface

```csharp
public interface IScrapedPriceRepository : IRepository<ScrapedPrice>
{
    Task<List<ScrapedPrice>> GetPriceHistoryByProductIdAsync(int productId, int take = 10);
    Task<ScrapedPrice> GetLatestPriceByProductIdAsync(int productId);
    Task<List<ScrapedPrice>> GetPricesByUrlAsync(string url);
}
```

## Service Interface

```csharp
public interface IScrapedPriceService
{
    Task<ScrapedPrice> ScrapePriceAsync(int productId, string sourceUrl);
    Task<List<ScrapedPrice>> GetPriceHistoryAsync(int productId, int take = 10);
    Task<ScrapedPrice> GetLatestPriceAsync(int productId);
    Task<List<ScrapedPrice>> GetPricesByUrlAsync(string url);
}
```

## File Locations

| Component | File |
|-----------|------|
| Entity | `Domain\Entities\ScrapedPrice.cs` |
| Repository Interface | `Application\Common\Interfaces\IScrapedPriceRepository.cs` |
| Repository | `Infrastructure\Repo\ScrapedPriceRepository.cs` |
| Service Interface | `Application\Services\Intrerfaces\IScrapedPriceService.cs` |
| Service | `Application\Services\Implementation\ScrapedPriceService.cs` |
| Scraper | `Infrastructure\PriceScraper\PriceScraperService.cs` |
| Migration | `Infrastructure\Migrations\20250101000000_AddScrapedPriceEntity.cs` |
| Example Controller | `Inventory_Management\Areas\Admin\Controllers\Examples\PriceScraperExampleController.cs` |

## Configuration

The service is automatically registered in DI. No additional configuration needed.

```csharp
// This is already done in DependencyInjection.cs
services.AddScoped<IScrapedPriceService, ScrapedPriceService>();
services.AddScoped<PriceScraperService>();
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| "Type not found" errors | Run `dotnet build` to restore packages |
| Database errors | Run `dotnet ef database update` |
| Service injection fails | Ensure migrations are applied first |
| Scraper returns "not implemented" | Install PuppeteerSharp or Selenium |
| No prices returned | Check `IsSuccessful` flag and `ErrorMessage` |

## Tips & Tricks

1. **Cache results** to avoid repeated scraping of same URL
2. **Schedule scraping** with Hangfire for automatic updates
3. **Log errors** for debugging failed scrapes
4. **Test selectors** in browser DevTools before scraping
5. **Use delays** between requests to be respectful
6. **Check robots.txt** before scraping
7. **Store user-agent** in config for flexibility
8. **Monitor rate limits** of target websites

## Common Selectors for Price Scraping

```javascript
// Meta tags
'meta[property="product:price:amount"]'
'meta[property="og:price:amount"]'
'meta[name="price"]'

// CSS classes
'.price'
'.price-value'
'.product-price'
'.sale-price'
'.current-price'

// Data attributes
'[data-price]'
'[data-cost]'
'[data-amount]'

// Common elements
'span.price'
'div.price'
'strong.price'
```

## Performance Tips

1. Use async/await for non-blocking operations
2. Batch scrape operations
3. Use connection pooling for database
4. Cache historical data
5. Index frequently queried columns (already done)
6. Use pagination for large result sets
7. Schedule scraping during low-traffic hours

## Security Considerations

1. Validate URLs before scraping
2. Sanitize stored URLs
3. Rate limit scraping operations
4. Use timeouts to prevent hanging
5. Log all scraping attempts
6. Implement IP rotation (if needed)
7. Respect website terms of service
