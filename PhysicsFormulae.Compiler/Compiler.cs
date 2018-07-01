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

    public enum ConstantSection
    {
        Definition = 1,
        Values = 2,
        References = 3,
        SeeMore = 4,
        Tags = 5
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

        public Constant CompileConstant(string[] lines)
        {
            lines = RemoveEmptyLines(lines);

            var constant = new Constant();

            constant.Reference = lines[0].Trim();
            constant.Title = lines[1].Trim();
            constant.Interpretation = lines[2].Trim();

            if (lines[3].Trim() == "universal")
            {
                constant.Type = ConstantType.Universal;
            }

            constant.Symbol = lines[4].Trim();

            var constantSection = ConstantSection.Definition;

            var value = new Value();
            var valueLine = 1;

            for (var n = 5; n < lines.Length; n++)
            {
                var line = lines[n].Trim();

                if (line == "values:")
                {
                    constantSection = ConstantSection.Values;
                    continue;
                }
                if (line == "references:")
                {
                    if (value.Coefficient != "" && value.Exponent != "")
                    {
                        constant.Values.Add(value);
                    }

                    constantSection = ConstantSection.References;
                    continue;
                }
                if (line == "see more:")
                {
                    constantSection = ConstantSection.SeeMore;
                    continue;
                }
                if (line == "tags:")
                {
                    constantSection = ConstantSection.Tags;
                    continue;
                }

                if (constantSection == ConstantSection.Values)
                {
                    if (line == "---")
                    {
                        if (value.Coefficient != "" && value.Exponent != "")
                        {
                            constant.Values.Add(value);
                        }

                        value = new Value();

                        valueLine = 1;

                        continue;
                    }
                    if (valueLine == 1)
                    {
                        value.Coefficient = line;
                        valueLine++;
                    }
                    else if (valueLine == 2)
                    {
                        value.Exponent = line;
                        valueLine++;
                    }
                    else if (valueLine == 3)
                    {
                        value.Units = line;
                        valueLine++;
                    }
                }

                if (constantSection == ConstantSection.References)
                {
                    if (IsLineWebpageReferenceLine(line))
                    {
                        var webpage = GetWebpageReference(line);

                        constant.References.Add(webpage);
                        continue;
                    }
                    if (IsLineBookReferenceLine(line))
                    {
                        var book = GetBookReference(line);

                        constant.References.Add(book);
                        continue;
                    }
                }

                if (constantSection == ConstantSection.SeeMore)
                {
                    if (IsSeeMoreLinkLine(line))
                    {
                        var seeMoreLink = GetSeeMoreLink(line);

                        constant.SeeMore.Add(seeMoreLink);
                        continue;
                    }
                }

                if (constantSection == ConstantSection.Tags)
                {
                    constant.Tags.Add(line);
                    continue;
                }
            }

            constant.URLReference = GetURLReference(constant.Reference);
            Autotag(constant);

            return constant;
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

            formula.URLReference = GetURLReference(formula.Reference);
            Autotag(formula);

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

        protected List<string> excludedWords = new List<string>() { "a", "an", "the", "in", "at", "on", "to", "from", "under", "over", "above", "below", "with", "without", "of", "it", "is", "are", "be", "which", "when", "what", "who", "how", "why", "where", "something", "through", "for", "along", "there", "must", "towards", "that", "and", "its", "know", "known", "travel", "fast", "faster", "than", "this", "carry", "carried", "by", "use", "used", "description", "describe", "described", "number", "include", "including", "", "", "", "", "", "" };

        private IEnumerable<string> Autotag(Formula formula)
        {

            var words = new List<string>();

            words.AddRange(GetWords(formula.Title));
            words.AddRange(GetWords(formula.Interpretation));

            foreach (var word in words)
            {
                if (formula.Tags.Any(t => t == word))
                {
                    continue;
                }

                if (excludedWords.Any(w => w == word))
                {
                    continue;
                }

                formula.Tags.Add(word);
            }

            return formula.Tags;
        }

        private IEnumerable<string> Autotag(Constant constant)
        {
            var words = new List<string>();

            words.AddRange(GetWords(constant.Title));
            words.AddRange(GetWords(constant.Interpretation));

            foreach (var word in words)
            {
                if (constant.Tags.Any(t => t == word))
                {
                    continue;
                }

                if (excludedWords.Any(w => w == word))
                {
                    continue;
                }

                constant.Tags.Add(word);
            }

            return constant.Tags;
        }

        private IEnumerable<string> GetWords(string text)
        {
            var normalisedText = text;

            normalisedText = normalisedText.ToLower();
            normalisedText = Regex.Replace(normalisedText, @"[\.\,\:\;\'\""\(\)\-]", "");

            return normalisedText.Split(' ');
        }

        private string GetURLReference(string reference)
        {
            var urlReference = reference;

            urlReference = Regex.Replace(urlReference, @"^(.+)([A-Z0-9])", @"$1-$2");
            urlReference = urlReference.ToLower();

            return urlReference;
        }
    }
}
