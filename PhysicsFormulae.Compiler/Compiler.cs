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
        DerivedFrom = 3,
        Fields = 4,
        References = 5,
        SeeMore = 6,
        Tags = 7
    }

    public class Compiler
    {
        public Formula CompileFormula(string[] lines)
        {
            lines = RemoveEmptyLines(lines);

            var formula = new Formula();

            formula.Reference = lines[0].Trim();
            formula.Title = lines[1].Trim();
            formula.Interpretation = lines[2].Trim();
            formula.Content = lines[3].Trim();

            var formulaSection = FormulaSection.Definition;

            for (var n = 4; n < lines.Length; n++)
            {
                var line = lines[n];

                if (line == "where:")
                {
                    formulaSection = FormulaSection.Where;
                    continue;
                }

                if (line == "derived from:")
                {
                    formulaSection = FormulaSection.DerivedFrom;
                    continue;
                }

                if (line == "fields:")
                {
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

        private bool IsLineIdentifierLine(string line)
        {
            var isMatch = Regex.IsMatch(line, @"^([^\[]+)\s*\[\s*(var\.|const\.)\s*([A-Za-z0-9_]+)?\s*\](.+)$");

            return isMatch;
        }

        private Identifier GetIdentifier(string line)
        {
            var identifier = new Identifier();

            var match = Regex.Match(line, @"^([^\[]+)\s*\[\s*(var\.|const\.)\s*([A-Za-z0-9_]+)?\s*\](.+)$");

            identifier.Content = match.Groups[1].Value.Trim();

            if (match.Groups[2].Value == "var.")
            {
                identifier.Type = IdentifierType.Variable;
            }
            else if (match.Groups[2].Value == "const.")
            {
                identifier.Type = IdentifierType.Constant;
            }

            identifier.Reference = match.Groups[3].Value.Trim();
            identifier.Definition = match.Groups[4].Value.Trim();

            return identifier;
        }

        private bool IsSeeMoreLinkLine(string line)
        {
            var isMatch = Regex.IsMatch(line, @"^(.+)\s+((http|https)://(.+))$");

            return isMatch;
        }

        private SeeMoreLink GetSeeMoreLink(string line)
        {
            var seeMoreLink = new SeeMoreLink();

            var match = Regex.Match(line, @"^(.+)\s+((http|https)://(.+))$");

            seeMoreLink.Name = match.Groups[1].Value.Trim();
            seeMoreLink.URL = match.Groups[2].Value.Trim();

            return seeMoreLink;
        }

        private bool IsLineWebpageReferenceLine(string line)
        {
            var isMatch = Regex.IsMatch(line, @"^webpage:\s*""([^""]+)""\s*,\s*([^,]+)\s*,\s*((http|https):\/\/[^\s]+)\s+\((\d{4}-\d{2}-\d{2})\)\s*$");

            return isMatch;
        }

        private Webpage GetWebpageReference(string line)
        {
            var webpage = new Webpage();

            var match = Regex.Match(line, @"^webpage:\s*""([^""]+)""\s*,\s*([^,]+)\s*,\s*((http|https):\/\/[^\s]+)\s+\((\d{4}-\d{2}-\d{2})\)\s*$");

            webpage.Title = match.Groups[1].Value.Trim();
            webpage.WebsiteTitle = match.Groups[2].Value.Trim();
            webpage.URL = match.Groups[3].Value.Trim();

            return webpage;
        }

        private bool IsLineBookReferenceLine(string line)
        {
            var isMatch = Regex.IsMatch(line, @"^book:\s*""([^""]+)""\s*,\s*([^,]+)\s*\((\d{1,3})\. Edition\)\s*\(([^\)]+)\)\s*ISBN\s+([0-9\-]+)\s*$");

            return isMatch;
        }

        private Book GetBookReference(string line)
        {
            var book = new Book();

            var match = Regex.Match(line, @"^book:\s*""([^""]+)""\s*,\s*([^,]+)\s*\((\d{1,3})\. Edition\)\s*\(([^\)]+)\)\s*ISBN\s+([0-9\-]+)\s*$");

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
