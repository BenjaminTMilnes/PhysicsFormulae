using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace PhysicsFormulae.Compiler
{
    public class SitemapURL
    {
        public string Location { get; set; }
        public DateTime LastModified { get; set; }
        public string Priority { get; set; }

        public SitemapURL(string location, DateTime lastModified, string priority)
        {
            Location = location;
            LastModified = lastModified;
            Priority = priority;
        }
    }

    public class Sitemap
    {
        public IList<SitemapURL> URLs { get; set; }

        public Sitemap()
        {
            URLs = new List<SitemapURL>();
        }

        public void AddURL(string location, DateTime lastModified, string priority)
        {
            URLs.Add(new SitemapURL(location, lastModified, priority));
        }

        public void AddURL(string location, string priority)
        {
            AddURL(location, DateTime.UtcNow, priority);
        }

        public void AddURL(string location)
        {
            AddURL(location, "1.0");
        }

        public void Save(string fileName)
        {
            var xmlNameSpace = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");
            var sitemap = new XDocument(new XDeclaration("1.0", "UTF-8", null));

            sitemap.Add(new XElement(xmlNameSpace + "urlset"));

            var root = sitemap.Root;

            foreach (var url1 in URLs)
            {
                var url = new XElement(xmlNameSpace + "url");

                var location = new XElement(xmlNameSpace + "loc");
                location.Add(new XText(url1.Location));

                var lastModifiedDate = new XElement(xmlNameSpace + "lastmod");
                lastModifiedDate.Add(new XText(url1.LastModified.ToString("yyyy-MM-dd")));

                var priority = new XElement(xmlNameSpace + "priority");
                priority.Add(new XText(url1.Priority));

                url.Add(location);
                url.Add(lastModifiedDate);
                url.Add(priority);

                root.Add(url);
            }

            sitemap.Save(fileName);
        }
    }
}
