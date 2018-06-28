using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PhysicsFormulae.Compiler
{
    public enum FormulaSection
    {
        Definition = 1,
        Where = 2,
        Variants = 3,
        DerivedFrom = 4,
        Fields = 5,
        References = 6,
        SeeMore = 7,
        Tags = 8
    }

    public enum ReferenceSection
    {
        CitationKey = 1,
        Title = 2,
        Authors = 3,
        Publishers = 4,
        Edition = 5,
        ISBN = 6
    }

    public class Compiler
    {
        public Reference CompileReference(string[] lines)
        {
            lines = RemoveEmptyLines(lines);

            if (lines[1].Trim() == "book")
            {
                var book = new Book();
                var authors = new List<string>();

                book.CitationKey = lines[0].Trim();

                var referenceSection = ReferenceSection.CitationKey;

                for (var n = 2; n < lines.Length; n++)
                {
                    var line = lines[n].Trim();

                    if (line == "title:")
                    {
                        referenceSection = ReferenceSection.Title;
                        continue;
                    }

                    if (line == "authors:")
                    {
                        referenceSection = ReferenceSection.Authors;
                        continue;
                    }

                    if (referenceSection == ReferenceSection.Title)
                    {
                        book.Title = line.Trim();
                    }

                    if (referenceSection == ReferenceSection.Authors)
                    {
                        authors.Add(line.Trim());
                    }

                    if (line.StartsWith("edition:"))
                    {
                        book.Edition = int.Parse(line.Substring(8));
                    }

                    if (line.StartsWith("publisher:"))
                    {
                        book.PublisherName = line.Substring(10);
                    }

                    if (line.StartsWith("isbn:"))
                    {
                        book.ISBN = line.Substring(5);
                    }
                }

                book.Authors = authors;

                return book;
            }

            throw new NotImplementedException();
        }

        public Formula CompileFormula(string[] lines)
        {
            lines = RemoveEmptyLines(lines);

            var formula = new Formula();

            formula.Reference = lines[0].Trim();
            formula.Title = lines[1].Trim();
            formula.Interpretation = lines[2].Trim();
            formula.Content = lines[3].Trim();

            var formulaSection = FormulaSection.Definition;

            var variant = new Variant();
            var variantLine = 1;

            for (var n = 4; n < lines.Length; n++)
            {
                var line = lines[n];

                if (line == "where:")
                {
                    formulaSection = FormulaSection.Where;
                    continue;
                }

                if (line == "variants:")
                {
                    formulaSection = FormulaSection.Variants;
                    continue;
                }

                if (line == "derived from:")
                {
                    formulaSection = FormulaSection.DerivedFrom;
                    continue;
                }

                if (line == "fields:")
                {
                    if (variant.Title != "" && variant.Content != "")
                    {
                        formula.Variants.Add(variant);
                    }

                    formulaSection = FormulaSection.Fields;
                    continue;
                }

                if (line == "references:")
                {
                    formulaSection = FormulaSection.References;
                    continue;
                }

                if (line == "see more:")
                {
                    formulaSection = FormulaSection.SeeMore;
                    continue;
                }

                if (line == "tags:")
                {
                    formulaSection = FormulaSection.Tags;
                    continue;
                }

                if (formulaSection == FormulaSection.Where)
                {
                    if (IsLineIdentifierLine(line))
                    {
                        var identifier = GetIdentifier(line);

                        formula.Identifiers.Add(identifier);
                        continue;
                    }
                }

                if (formulaSection == FormulaSection.Variants)
                {
                    if (line == "---")
                    {
                        if (variant.Title != "" && variant.Content != "")
                        {
                            formula.Variants.Add(variant);
                        }

                        variant = new Variant();

                        variantLine = 1;

                        continue;
                    }

                    if (variantLine == 1)
                    {
                        variant.Title = line;
                        variantLine++;
                    }
                    else if (variantLine == 2)
                    {
                        variant.Content = line;
                        variantLine++;
                    }
                    else if (variantLine == 3)
                    {
                        variant.Interpretation = line;
                        variantLine++;
                    }
                }

                if (formulaSection == FormulaSection.DerivedFrom)
                {
                    formula.DerivedFrom.Add(line);
                    continue;
                }

                if (formulaSection == FormulaSection.Fields)
                {
                    formula.Fields.Add(line);
                    continue;
                }

                if (formulaSection == FormulaSection.References)
                {
                    if (IsLineWebpageReferenceLine(line))
                    {
                        var webpage = GetWebpageReference(line);

                        formula.References.Add(webpage);
                        continue;
                    }
                    if (IsLineBookReferenceLine(line))
                    {
                        var book = GetBookReference(line);

                        formula.References.Add(book);
                        continue;
                    }
                }

                if (formulaSection == FormulaSection.SeeMore)
                {
                    if (IsSeeMoreLinkLine(line))
                    {
                        var seeMoreLink = GetSeeMoreLink(line);

                        formula.SeeMore.Add(seeMoreLink);
                        continue;
                    }
                }

                if (formulaSection == FormulaSection.Tags)
                {
                    formula.Tags.Add(line);
                    continue;
                }
            }

            return formula;
        }

        private string[] RemoveEmptyLines(string[] lines)
        {
            return lines.Select(l => l.Trim()).Where(l => l != "").ToArray();
        }

        protected string _identifierPattern = @"^([^\[]+)\s*\[\s*(var\.|const\.)\s*(scal\.|vec\.|matr\.|tens|.)?\s*([A-Za-z0-9_]+)?\s*\](.+)$";
        protected string _seeMoreLinkPattern = @"^(.+)\s+((http|https)://(.+))$";
        protected string _urlPattern = @"^(https?://[^\s]+)$";
        protected string _webpageReferencePattern = @"^webpage:\s*""([^""]+)""\s*,\s*([^,]+)\s*,\s*((http|https):\/\/[^\s]+)\s+\((\d{4}-\d{2}-\d{2})\)\s*$";
        protected string _bookCitationPattern = @"^book:\s*([A-Za-z0-9_]+)$";
        protected string _bookReferencePattern = @"^book:\s*""([^""]+)""\s*,\s*([^,]+)\s*\((\d{1,3})\. Edition\)\s*\(([^\)]+)\)\s*ISBN\s+([0-9\-]+)\s*$";

        private bool IsLineIdentifierLine(string line)
        {
            return Regex.IsMatch(line, _identifierPattern);
        }

        private Identifier GetIdentifier(string line)
        {
            var identifier = new Identifier();

            var match = Regex.Match(line, _identifierPattern);

            identifier.Content = match.Groups[1].Value.Trim();

            if (match.Groups[2].Value == "var.")
            {
                identifier.Type = IdentifierType.Variable;
            }
            else if (match.Groups[2].Value == "const.")
            {
                identifier.Type = IdentifierType.Constant;
            }

            if (match.Groups[3].Value == "scal.")
            {
                identifier.ObjectType = ObjectType.Scalar;
            }
            else if (match.Groups[3].Value == "vec.")
            {
                identifier.ObjectType = ObjectType.Vector;
            }
            else if (match.Groups[3].Value == "matr.")
            {
                identifier.ObjectType = ObjectType.Matrix;
            }
            else if (match.Groups[3].Value == "tens.")
            {
                identifier.ObjectType = ObjectType.Tensor;
            }

            identifier.Reference = match.Groups[4].Value.Trim();
            identifier.Definition = match.Groups[5].Value.Trim();

            return identifier;
        }

        private bool IsSeeMoreLinkLine(string line)
        {
            return (Regex.IsMatch(line, _seeMoreLinkPattern) || Regex.IsMatch(line, _urlPattern));
        }

        private SeeMoreLink GetSeeMoreLink(string line)
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

                return seeMoreLink;
            }

            var match = Regex.Match(line, _seeMoreLinkPattern);

            seeMoreLink.Name = match.Groups[1].Value.Trim();
            seeMoreLink.URL = match.Groups[2].Value.Trim();

            return seeMoreLink;
        }

        private bool IsLineWebpageReferenceLine(string line)
        {
            return Regex.IsMatch(line, _webpageReferencePattern);
        }

        private Webpage GetWebpageReference(string line)
        {
            var webpage = new Webpage();

            var match = Regex.Match(line, _webpageReferencePattern);

            webpage.Title = match.Groups[1].Value.Trim();
            webpage.WebsiteTitle = match.Groups[2].Value.Trim();
            webpage.URL = match.Groups[3].Value.Trim();

            return webpage;
        }

        private bool IsLineBookCitationLine(string line)
        {
            return Regex.IsMatch(line, _bookCitationPattern);
        }

        private string GetBookCitation(string line)
        {
            var match = Regex.Match(line, _bookCitationPattern);

            return match.Groups[1].Value.Trim();
        }

        private bool IsLineBookReferenceLine(string line)
        {
            return Regex.IsMatch(line, _bookReferencePattern);
        }

        private Book GetBookReference(string line)
        {
            var book = new Book();

            var match = Regex.Match(line, _bookReferencePattern);

            book.Title = match.Groups[1].Value.Trim();
            book.Authors = match.Groups[2].Value.Split(new string[] { "and" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
            book.Edition = int.Parse(match.Groups[3].Value.Trim());
            //book.Year = int.Parse(match.Groups[4].Value.Trim());
            book.PublisherName = match.Groups[4].Value.Trim();
            book.ISBN = match.Groups[5].Value.Trim();

            return book;
        }
    }
}
