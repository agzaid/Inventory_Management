using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Xml.Linq;
using Application.Services.Intrerfaces;

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

        private static readonly XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

        [HttpGet("sitemap.xml")]
        public async Task<IActionResult> Index()
        {
            string baseUrl = $"{Request.Scheme}://{Request.Host}";

            var sitemaps = new List<XElement>
            {
                new XElement(ns + "sitemap",
                    new XElement(ns + "loc", $"{baseUrl}/sitemap-static.xml"),
                    new XElement(ns + "lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd"))
                ),
                new XElement(ns + "sitemap",
                    new XElement(ns + "loc", $"{baseUrl}/sitemap-categories.xml"),
                    new XElement(ns + "lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd"))
                )
            };

            var products = await _productService.GetAllProducts();
            if (products != null && products.Any())
            {
                const int pageSize = 5000;
                int totalPages = (int)Math.Ceiling(products.Count() / (double)pageSize);

                for (int i = 1; i <= totalPages; i++)
                {
                    sitemaps.Add(new XElement(ns + "sitemap",
                        new XElement(ns + "loc", $"{baseUrl}/sitemap-products-{i}.xml"),
                        new XElement(ns + "lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd"))
                    ));
                }
            }

            var sitemapIndex = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(ns + "sitemapindex", sitemaps)
            );

            var xmlString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + sitemapIndex.ToString(SaveOptions.DisableFormatting);
            return Content(xmlString, "application/xml", Encoding.UTF8);
        }
        [HttpGet("sitemap-static.xml")]
        public IActionResult Static()
        {
            string baseUrl = $"{Request.Scheme}://{Request.Host}";

            var urls = new List<XElement>
            {
                CreateUrl($"{baseUrl}/", "weekly", "1.0")
            };

            var sitemap = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(ns + "urlset", urls)
            );

            var xmlString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + sitemap.ToString(SaveOptions.DisableFormatting);
            return Content(xmlString, "application/xml", Encoding.UTF8);
        }

        [HttpGet("sitemap-categories.xml")]
        public async Task<IActionResult> Categories()
        {
            string baseUrl = $"{Request.Scheme}://{Request.Host}";

            var categoriesResult = _categoryService.GetAllCategories();
            var urls = new List<XElement>();

            if (categoriesResult != null)
            {
                foreach (var c in categoriesResult)
                {
                    urls.Add(CreateUrl($"{baseUrl}/category/{c.Slug}", "weekly", "0.7"));
                }
            }

            var sitemap = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(ns + "urlset", urls)
            );

            var xmlString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + sitemap.ToString(SaveOptions.DisableFormatting);
            return Content(xmlString, "application/xml", Encoding.UTF8);
        }
        [HttpGet("sitemap-products-{page}.xml")]
        public async Task<IActionResult> Products(int page)
        {
            const int pageSize = 5000;
            string baseUrl = $"{Request.Scheme}://{Request.Host}";

            var products = await _productService.GetAllProducts();
            var urls = new List<XElement>();

            if (products != null && products.Any())
            {
                var paged = products
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                foreach (var p in paged)
                {
                    urls.Add(CreateUrl($"{baseUrl}/product/details/{p.Slug}", "weekly", "0.9"));
                }
            }

            var sitemap = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                new XElement(ns + "urlset", urls)
            );


            var xmlString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + sitemap.ToString(SaveOptions.DisableFormatting);
            return Content(xmlString, "application/xml", Encoding.UTF8);
        }

        private XElement CreateUrl(string loc, string changefreq, string priority)
        {
            return new XElement(ns + "url",
                new XElement(ns + "loc", loc),
                new XElement(ns + "lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd")),
                new XElement(ns + "changefreq", changefreq),
                new XElement(ns + "priority", priority)
            );
        }
    }
}
