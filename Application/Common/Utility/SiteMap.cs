using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Application.Common.Utility
{
    public static class SitemapBuilder
    {
        /// <summary>
        /// Builds an XML sitemap given a collection of URLs.
        /// </summary>
        public static string BuildSitemapXml(IEnumerable<(string loc, DateTime lastmod, double priority)> urls)
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

            var sitemap = new XElement(ns + "urlset",
                urls.Select(u =>
                    new XElement(ns + "url",
                        new XElement(ns + "loc", u.loc),
                        new XElement(ns + "lastmod", u.lastmod.ToString("yyyy-MM-dd")),
                        new XElement(ns + "changefreq", "weekly"),
                        new XElement(ns + "priority", u.priority.ToString("F1"))
                    )
                )
            );

            var xml = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), sitemap);
            return xml.ToString();
        }
    }
}
