using System;
using System.Linq;
using System.Text.RegularExpressions;
using PhysicsFormulae.Compiler.References;

namespace PhysicsFormulae.Compiler
{
    public abstract class Compiler
    {
        protected ReferenceConverter _referenceConverter;
        protected Autotagger _autotagger;

        public Compiler(Autotagger autotagger)
        {
            _referenceConverter = new ReferenceConverter();
            _autotagger = autotagger;
        }

        protected string[] RemoveEmptyLines(string[] lines)
        {
            return lines.Select(l => l.Trim()).Where(l => l != "").ToArray();
        }

        protected string _seeMoreLinkPattern = @"^(.+)\s+((http|https)://(.+))$";
        protected string _urlPattern = @"^(https?://[^\s]+)$";

        protected string _webpageReferencePattern = @"^webpage:\s*""([^""]+)""\s*,\s*([^,]+)\s*,\s*((http|https):\/\/[^\s]+)\s+\((\d{4}-\d{2}-\d{2})\)\s*$";

        protected string _bookCitationPattern = @"^book:\s*([A-Za-z0-9_]+)$";
        protected string _bookReferencePattern = @"^book:\s*""([^""]+)""\s*,\s*([^,]+)\s*\((\d{1,3})\. Edition\)\s*\(([^\)]+)\)\s*ISBN\s+([0-9\-]+)\s*$";

        public bool IsSeeMoreLinkLine(string line)
        {
            return (Regex.IsMatch(line, _seeMoreLinkPattern) || Regex.IsMatch(line, _urlPattern));
        }

        public SeeMoreLink GetSeeMoreLink(string line)
        {
            var seeMoreLink = new SeeMoreLink();

            if (Regex.IsMatch(line, _urlPattern))
            {
                seeMoreLink.URL = line.Trim();

                if (seeMoreLink.URL.Contains("en.wikipedia.org"))
                {
                    seeMoreLink.Name = "Wikipedia";
                }
                if (seeMoreLink.URL.Contains("hyperphysics.phy-astr.gsu.edu"))
                {
                    seeMoreLink.Name = "Hyperphysics";
                }
                if (seeMoreLink.URL.Contains("physics.info"))
                {
                    seeMoreLink.Name = "The Physics Hypertextbook";
                }

                return seeMoreLink;
            }

            var match = Regex.Match(line, _seeMoreLinkPattern);

            seeMoreLink.Name = match.Groups[1].Value.Trim();
            seeMoreLink.URL = match.Groups[2].Value.Trim();

            return seeMoreLink;
        }

        public bool IsLineWebpageReferenceLine(string line)
        {
            return Regex.IsMatch(line, _webpageReferencePattern);
        }

        public Webpage GetWebpageReference(string line)
        {
            var webpage = new Webpage();

            var match = Regex.Match(line, _webpageReferencePattern);

            webpage.Title = match.Groups[1].Value.Trim();
            webpage.WebsiteTitle = match.Groups[2].Value.Trim();
            webpage.URL = match.Groups[3].Value.Trim();

            return webpage;
        }

        public bool IsLineBookCitationLine(string line)
        {
            return Regex.IsMatch(line, _bookCitationPattern);
        }

        public string GetBookCitation(string line)
        {
            var match = Regex.Match(line, _bookCitationPattern);

            return match.Groups[1].Value.Trim();
        }

        public bool IsLineBookReferenceLine(string line)
        {
            return Regex.IsMatch(line, _bookReferencePattern);
        }

        public Book GetBookReference(string line)
        {
            var book = new Book();

            var match = Regex.Match(line, _bookReferencePattern);

            book.Title = match.Groups[1].Value.Trim();
            book.Authors = match.Groups[2].Value.Split(new string[] { "and" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
            book.Edition = int.Parse(match.Groups[3].Value.Trim());
            book.PublisherName = match.Groups[4].Value.Trim();
            book.ISBN = match.Groups[5].Value.Trim();

            return book;
        }
    }
}
