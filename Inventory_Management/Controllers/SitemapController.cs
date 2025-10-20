using Application.Common.Utility;
using Application.Services.Intrerfaces;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml.Linq;

namespace Inventory_Management.Controllers
{
    public class SitemapController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public SitemapController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [HttpGet("sitemap.xml")]
        public async Task<IActionResult> Index()
        {
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            var sitemaps = new List<XElement>();

            // Static sitemap (main pages)
            sitemaps.Add(new XElement("sitemap",
                new XElement("loc", $"{baseUrl}/sitemap-static.xml"),
                new XElement("lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd"))
            ));

            // Categories sitemap
            sitemaps.Add(new XElement("sitemap",
                new XElement("loc", $"{baseUrl}/sitemap-categories.xml"),
                new XElement("lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd"))
            ));

            // Product sitemaps (paginate if large)
            var result = await _productService.GetAllProducts();
            if (result != null)
            {
                const int pageSize = 5000; // Google recommends max 50k URLs per sitemap
                int totalPages = (int)Math.Ceiling(result.ToList().Count / (double)pageSize);

                for (int i = 1; i <= totalPages; i++)
                {
                    sitemaps.Add(new XElement("sitemap",
                        new XElement("loc", $"{baseUrl}/sitemap-products-{i}.xml"),
                        new XElement("lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd"))
                    ));
                }
            }

            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var sitemapIndex = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "sitemapindex", sitemaps)
            );

            return Content(sitemapIndex.ToString(), "application/xml", Encoding.UTF8);
        }

        // 🟩 Static pages sitemap
        [HttpGet("sitemap-static.xml")]
        public IActionResult Static()
        {
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            var urls = new List<XElement>
            {
                CreateUrl($"{baseUrl}/", "weekly", "1.0"),
                CreateUrl($"{baseUrl}/about", "monthly", "0.6"),
                CreateUrl($"{baseUrl}/contact", "monthly", "0.6"),
                CreateUrl($"{baseUrl}/shop", "weekly", "0.8")
            };

            var sitemap = new XDocument(new XElement(ns + "urlset", urls));
            return Content(sitemap.ToString(), "application/xml", Encoding.UTF8);
        }

        // 🟩 Categories sitemap
        [HttpGet("sitemap-categories.xml")]
        public async Task<IActionResult> Categories()
        {
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            var categoriesResult = _categoryService.GetAllCategories();
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            var urls = new List<XElement>();
            if (categoriesResult != null)
            {
                foreach (var c in categoriesResult)
                {
                    urls.Add(CreateUrl($"{baseUrl}/category/{c.Slug}", "weekly", "0.7"));
                }
            }

            var sitemap = new XDocument(new XElement(ns + "urlset", urls));
            return Content(sitemap.ToString(), "application/xml", Encoding.UTF8);
        }

        // 🟩 Paginated product sitemap
        [HttpGet("sitemap-products-{page}.xml")]
        public async Task<IActionResult> Products(int page)
        {
            const int pageSize = 5000;
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            var productsResult = await _productService.GetAllProducts();
            var urls = new List<XElement>();

            if (productsResult != null)
            {
                var products = productsResult
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                foreach (var p in products)
                {
                    urls.Add(CreateUrl($"{baseUrl}/product/details/{p.Slug}", "weekly", "0.9"));
                }
            }

            var sitemap = new XDocument(new XElement(ns + "urlset", urls));
            return Content(sitemap.ToString(), "application/xml", Encoding.UTF8);
        }

        // Helper
        private static XElement CreateUrl(string loc, string changefreq, string priority)
        {
            return new XElement("url",
                new XElement("loc", loc),
                new XElement("lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd")),
                new XElement("changefreq", changefreq),
                new XElement("priority", priority)
            );
        }
    }
}