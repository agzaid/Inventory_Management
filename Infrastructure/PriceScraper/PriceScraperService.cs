using PuppeteerSharp;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Application.Common.Interfaces;
using Domain.Entities;

namespace Infrastructure.PriceScraper
{
    /// <summary>
    /// Price scraper service using PuppeteerSharp for JavaScript-heavy sites
    /// and Selenium WebDriver as fallback for DOM-based scraping.
    /// </summary>
    public class PriceScraperService
    {
        private static IBrowser _browser;
        private static readonly object _browserLock = new object();
        private static bool _browserInitializing = false;
        private static readonly string BrowserCachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PuppeteerBrowsers");

        private readonly IUnitOfWork _unitOfWork;

        public PriceScraperService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private async Task<IBrowser> GetBrowserInstanceAsync()
        {
            // ✅ OPTIMIZATION 1: Quick exit if browser is already ready
            if (_browser != null && !_browser.IsClosed)
            {
                return _browser;
            }

            // ✅ OPTIMIZATION 2: Use lock to prevent multiple concurrent initializations
            lock (_browserLock)
            {
                // Double-check pattern: verify again inside lock
                if (_browser != null && !_browser.IsClosed)
                {
                    return _browser;
                }

                // Prevent multiple threads from downloading simultaneously
                if (_browserInitializing)
                {
                    // Wait for initialization to complete
                    while (_browserInitializing && (_browser == null || _browser.IsClosed))
                    {
                        System.Threading.Monitor.Wait(_browserLock, 500); // Wait up to 500ms between checks
                    }
                    return _browser;
                }

                _browserInitializing = true;
            }

            try
            {
                // ✅ OPTIMIZATION 3: Check if Chromium is already cached BEFORE attempting download
                var fetcher = new BrowserFetcher(new BrowserFetcherOptions { Path = BrowserCachePath });
                var installed = fetcher.GetInstalledBrowsers().FirstOrDefault();

                // Only download if NOT already cached
                if (installed == null)
                {
                    Console.WriteLine("📥 Chromium not found locally. Downloading (~150MB, one-time operation)...");
                    await fetcher.DownloadAsync();
                    installed = fetcher.GetInstalledBrowsers().FirstOrDefault()
                        ?? throw new Exception("Chromium could not be initialized. Check disk space and permissions.");
                    Console.WriteLine("✅ Chromium download complete!");
                }
                else
                {
                    Console.WriteLine("✅ Using cached Chromium browser");
                }

                // ✅ OPTIMIZATION 4: Launch with better performance settings
                _browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    ExecutablePath = installed.GetExecutablePath(),
                    Args = new[]
                    {
                        "--no-sandbox",
                        "--disable-gpu",
                        "--disable-dev-shm-usage",
                        "--disable-sync",                    // Don't sync anything
                        "--disable-background-networking",   // Disable background network
                        "--disable-extensions",              // Don't load extensions
                        "--disable-component-update",        // Don't auto-update
                        "--single-process"                   // Single process mode (faster for scraping)
                    },
                    Timeout = 10000  // 10 second timeout for launch
                });

                Console.WriteLine("✅ Browser instance created successfully");
                return _browser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Browser initialization failed: {ex.Message}");
                throw;
            }
            finally
            {
                lock (_browserLock)
                {
                    _browserInitializing = false;
                    System.Threading.Monitor.PulseAll(_browserLock); // Signal waiting threads
                }
            }
        }
        public async Task<ScrapeResult> ScrapeWithPuppeteerAsync(string url)
        {
            IBrowser browser = await GetBrowserInstanceAsync();
            IPage page = null;

            try
            {
                page = await browser.NewPageAsync();

                // ✅ OPTIMIZATION 1: Set viewport to skip unnecessary rendering
                await page.SetViewportAsync(new ViewPortOptions { Width = 1280, Height = 720 });

                // ✅ OPTIMIZATION 2: Block resource types that slow down scraping
                await page.SetRequestInterceptionAsync(true);
                page.Request += async (sender, e) =>
                {
                    var type = e.Request.ResourceType;
                    // Block: images, CSS, fonts, media, tracking
                    if (type == ResourceType.Image ||
                        type == ResourceType.StyleSheet ||
                        type == ResourceType.Font ||
                        type == ResourceType.Media ||
                        type == ResourceType.Other && e.Request.Url.Contains("analytics"))
                    {
                        await e.Request.AbortAsync();
                    }
                    else
                    {
                        await e.Request.ContinueAsync();
                    }
                };

                await page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

                // ✅ OPTIMIZATION 3: Use shorter timeout and DOMContentLoaded instead of NetworkIdle
                try
                {
                    await page.GoToAsync(url, new NavigationOptions
                    {
                        WaitUntil = new[] { WaitUntilNavigation.DOMContentLoaded },
                        Timeout = 15000  // 15 second timeout (reduced from 20)
                    });
                }
                catch (NavigationException)
                {
                    // Page might still be usable even if navigation times out
                    Console.WriteLine("⚠️ Navigation timeout, attempting to extract price anyway...");
                }

                // ✅ OPTIMIZATION 4: Parallel extraction (extract price while waiting)
                var priceExtractTask = ExtractPriceWithPuppeteerAsync(page);

                // Use shorter timeout for price element detection
                try
                {
                    await Task.WhenAny(
                        page.WaitForSelectorAsync(".flex.items-baseline.force-ltr, meta[property='product:price:amount']",
                            new WaitForSelectorOptions { Timeout = 3000 }),
                        Task.Delay(3000)
                    );
                }
                catch
                {
                    // Continue anyway - element might not exist but price could still be extracted
                }

                var priceData = await priceExtractTask;

                if (!string.IsNullOrEmpty(priceData) &&
                    decimal.TryParse(priceData, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                {
                    return new ScrapeResult
                    {
                        Success = true,
                        Price = price,
                        Method = "Puppeteer",
                        Url = url,
                        Currency = "EGP",
                        Timestamp = DateTime.UtcNow
                    };
                }

                return new ScrapeResult
                {
                    Success = false,
                    Error = "Price parsing failed",
                    Url = url,
                    Method = "Puppeteer"
                };
            }
            catch (Exception ex)
            {
                return new ScrapeResult
                {
                    Success = false,
                    Error = $"Puppeteer error: {ex.Message}",
                    Url = url,
                    Method = "Puppeteer"
                };
            }
            finally
            {
                // ✅ CRITICAL: Close the PAGE only (reuse browser instance for next requests)
                if (page != null)
                {
                    try
                    {
                        await page.CloseAsync();
                    }
                    catch { }
                }
            }
        }
        private async Task<string> ExtractPriceWithPuppeteerAsync(IPage page)
        {
            var priceData = await page.EvaluateFunctionAsync<string>(@"() => {
                // Method 1: Check for Open Graph product price meta tag
                const metaPrice = document.querySelector('meta[property=""product:price:amount""]');
                if (metaPrice && metaPrice.content) {
                    return metaPrice.content;
                }

                // Method 2: Check for other common price meta tags
                const priceSelectors = [
                    'meta[property=""og:price:amount""]',
                    'meta[name=""price""]',
                    'meta[property=""price""]'
                ];
                
                for (const selector of priceSelectors) {
                    const element = document.querySelector(selector);
                    if (element && element.content) {
                        return element.content;
                    }
                }

                // Method 3: Carrefour-specific selectors - Updated with exact HTML structure
                const carrefourSelectors = [
                    '.flex.items-baseline.force-ltr', // Main price container
                    '.product-details-price .price',
                    '.price-current',
                    '.current-price',
                    '.product-price .price',
                    'span.price',
                    'div.price',
                    '[data-qa=""price""]',
                    '.price-value',
                    '.sale-price'
                ];

                for (const selector of carrefourSelectors) {
                    const element = document.querySelector(selector);
                    if (element && element.textContent) {
                        const priceText = element.textContent.trim();
                        
                        // Special handling for Carrefour's split price structure
                        if (selector === '.flex.items-baseline.force-ltr') {
                            // Extract the full price from the container
                            // Look for pattern like ""42"" + "".99"" in the same container
                            const mainPriceMatch = priceText.match(/(\d+)\s*\.?\s*(\d{2})?/);
                            if (mainPriceMatch) {
                                const wholePart = mainPriceMatch[1];
                                const decimalPart = mainPriceMatch[2] || '00';
                                return `${wholePart}.${decimalPart}`;
                            }
                        }
                        
                        // Standard price extraction for other selectors
                        const priceMatch = priceText.match(/(\d+\.?\d*)/);
                        if (priceMatch && parseFloat(priceMatch[1]) > 1) { // Filter out small numbers like ""8""
                            return priceMatch[1].replace(/,/g, '');
                        }
                    }
                }

                // Method 3.5: Special Carrefour price extraction - combine the split parts
                const priceContainer = document.querySelector('.flex.items-baseline.force-ltr');
                if (priceContainer) {
                    const priceDivs = priceContainer.querySelectorAll('div');
                    let wholeNumber = '';
                    let decimalNumber = '';
                    
                    for (const div of priceDivs) {
                        const text = div.textContent.trim();
                        // Look for the main price number (e.g., ""42"")
                        if (/^\d+$/.test(text) && parseInt(text) > 1) {
                            wholeNumber = text;
                        }
                        // Look for decimal part (e.g., "".99"")
                        if (/^\.\d+$/.test(text)) {
                            decimalNumber = text.substring(1); // Remove the dot
                        }
                    }
                    
                    if (wholeNumber && decimalNumber) {
                        return `${wholeNumber}.${decimalNumber}`;
                    } else if (wholeNumber) {
                        return `${wholeNumber}.00`;
                    }
                }

                // Method 4: Look for schema.org structured data
                const jsonScripts = document.querySelectorAll('script[type=""application/ld+json""]');
                for (const script of jsonScripts) {
                    try {
                        const data = JSON.parse(script.textContent);
                        if (data.offers && data.offers.price) {
                            return data.offers.price.toString();
                        }
                        if (data[0] && data[0].offers && data[0].offers.price) {
                            return data[0].offers.price.toString();
                        }
                    } catch (e) {
                        // Ignore JSON parsing errors
                    }
                }

                // Method 5: Look for elements containing ""EGP"" followed by price
                const egpElements = document.querySelectorAll('*');
                for (const element of egpElements) {
                    if (element.textContent && element.textContent.includes('EGP')) {
                        const priceMatch = element.textContent.match(/EGP\s*(\d+\.?\d*)/);
                        if (priceMatch && parseFloat(priceMatch[1]) > 1) {
                            return priceMatch[1].replace(/,/g, '');
                        }
                    }
                }

                // Method 6: Look for decimal prices (most reliable for product prices)
                const allElements = document.querySelectorAll('span, div, b, strong, p');
                for (const element of allElements) {
                    if (element.textContent) {
                        const text = element.textContent.trim();
                        // Look for patterns like ""42.99"", ""42,99"", etc.
                        const priceMatch = text.match(/\b(\d{1,4}[.,]\d{2})\b/);
                        if (priceMatch && parseFloat(priceMatch[1].replace(',', '.')) > 1) {
                            return priceMatch[1].replace(',', '.');
                        }
                    }
                }

                return null;
            }");

            return priceData;
        }

        public ScrapeResult ScrapeWithSelenium(string url)
        {
            try
            {
                var options = new ChromeOptions();
                options.AddArgument("--headless");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--disable-gpu");
                options.AddUserProfilePreference("managed_default_content_settings.images", 2);
                options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36");

                using var driver = new ChromeDriver(options);
                driver.Navigate().GoToUrl(url);

                // Wait for page to load
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.FindElement(By.TagName("body")));

                var priceData = ExtractPriceWithSelenium(driver);

                if (decimal.TryParse(priceData, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                {
                    return new ScrapeResult
                    {
                        Success = true,
                        Price = price,
                        Currency = "EGP",
                        Timestamp = DateTime.UtcNow,
                        Method = "Selenium",
                        Url = url
                    };
                }
                else
                {
                    return new ScrapeResult
                    {
                        Success = false,
                        Error = "Could not extract price with Selenium",
                        Method = "Selenium",
                        Url = url
                    };
                }
            }
            catch (Exception ex)
            {
                return new ScrapeResult
                {
                    Success = false,
                    Error = $"Selenium scrape failed: {ex.Message}",
                    Method = "Selenium",
                    Url = url
                };
            }
        }

        private string ExtractPriceWithSelenium(IWebDriver driver)
        {
            // Method 1: Check for meta tags
            var metaSelectors = new[]
            {
                "meta[property='product:price:amount']",
                "meta[property='og:price:amount']",
                "meta[name='price']",
                "meta[property='price']"
            };

            foreach (var selector in metaSelectors)
            {
                try
                {
                    var element = driver.FindElement(By.CssSelector(selector));
                    var content = element.GetAttribute("content");
                    if (!string.IsNullOrEmpty(content))
                    {
                        return content;
                    }
                }
                catch (NoSuchElementException)
                {
                    // Continue to next selector
                }
            }

            // Method 2: Carrefour-specific CSS selectors
            var carrefourSelectors = new[]
            {
                ".flex.items-baseline.force-ltr",
                ".product-details-price .price",
                ".price-current",
                ".current-price",
                ".product-price .price",
                "span.price",
                "div.price",
                "[data-qa='price']",
                ".price-value",
                ".sale-price"
            };

            foreach (var selector in carrefourSelectors)
            {
                try
                {
                    var element = driver.FindElement(By.CssSelector(selector));
                    var text = element.Text;
                    if (!string.IsNullOrEmpty(text))
                    {
                        if (selector == ".flex.items-baseline.force-ltr")
                        {
                            var mainPriceMatch = System.Text.RegularExpressions.Regex.Match(text, @"(\d+)\s*\.?\s*(\d{2})?");
                            if (mainPriceMatch.Success)
                            {
                                var wholePart = mainPriceMatch.Groups[1].Value;
                                var decimalPart = mainPriceMatch.Groups[2].Value;
                                if (string.IsNullOrEmpty(decimalPart)) decimalPart = "00";
                                return $"{wholePart}.{decimalPart}";
                            }
                        }

                        var priceMatch = System.Text.RegularExpressions.Regex.Match(text, @"(\d+\.?\d*)");
                        if (priceMatch.Success && decimal.Parse(priceMatch.Value, System.Globalization.NumberStyles.Any) > 1)
                        {
                            return priceMatch.Value.Replace(",", "");
                        }
                    }
                }
                catch (NoSuchElementException)
                {
                    // Continue to next selector
                }
            }

            // Method 3: Look for elements containing "EGP" followed by price
            var egpElements = driver.FindElements(By.XPath("//*[contains(text(),'EGP')]"));
            foreach (var element in egpElements)
            {
                try
                {
                    var text = element.Text;
                    var priceMatch = System.Text.RegularExpressions.Regex.Match(text, @"EGP\s*(\d+\.?\d*)");
                    if (priceMatch.Success && decimal.Parse(priceMatch.Groups[1].Value, System.Globalization.NumberStyles.Any) > 1)
                    {
                        return priceMatch.Groups[1].Value.Replace(",", "");
                    }
                }
                catch
                {
                    // Continue on error
                }
            }

            // Method 4: Look for decimal prices
            var allElements = driver.FindElements(By.CssSelector("span, div, b, strong, p"));
            foreach (var element in allElements)
            {
                try
                {
                    var text = element.Text?.Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
                        var priceMatch = System.Text.RegularExpressions.Regex.Match(text, @"\b(\d{1,4}[.,]\d{2})\b");
                        if (priceMatch.Success)
                        {
                            var priceValue = priceMatch.Groups[1].Value.Replace(",", ".");
                            if (decimal.Parse(priceValue, System.Globalization.NumberStyles.Any) > 1)
                            {
                                return priceValue;
                            }
                        }
                    }
                }
                catch
                {
                    // Continue on error
                }
            }

            return string.Empty;
        }

        public async Task<ScrapeResult> ScrapeAsync(string url, bool useSeleniumFallback = true)
        {
            Console.WriteLine("🚀 Attempting scrape with Puppeteer...");
            var result = await ScrapeWithPuppeteerAsync(url);

            if (!result.Success && useSeleniumFallback)
            {
                Console.WriteLine($"⚠️ Puppeteer failed: {result.Error}. Falling back to Selenium...");
                result = ScrapeWithSelenium(url);
            }

            return result;
        }

        public async Task<ScrapeResult> ScrapeAsync(int productId, string url, bool useSeleniumFallback = true)
        {
            var result = await ScrapeAsync(url, useSeleniumFallback);

            if (result.Success)
            {
                await SaveScrapeResultToDatabase(productId, url, result);
            }

            return result;
        }

        private async Task SaveScrapeResultToDatabase(int productId, string url, ScrapeResult result)
        {
            try
            {
                var scrapedPrice = new ScrapedPrice
                {
                    ProductId = productId,
                    SourceUrl = url,
                    Price = result.Price,
                    Currency = result.Currency,
                    ScraperMethod = result.Method,
                    IsSuccessful = true,
                    ErrorMessage = string.Empty,
                    ScrapedDateTime = result.Timestamp
                };

                await _unitOfWork.ScrapedPrice.AddAsync(scrapedPrice);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

    }

    public class ScrapeResult
    {
        public bool Success { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = "EGP";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Method { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}
