using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using PhysicsFormulae.Compiler.References;

namespace PhysicsFormulae.Compiler.Formulae
{
    public enum FormulaSection
    {
        Definition = 1,
        Where = 2,
        Variants = 3,
        DerivedFrom = 4,
        Derivation = 5,
        Fields = 6,
        References = 7,
        SeeMore = 8,
        Tags = 9,
        Curricula = 10,
        Rating = 11
    }

    public class FormulaCompiler : Compiler
    {
        public FormulaCompiler(Autotagger autotagger) : base(autotagger) { }

        protected string _identifierPattern = @"^([^\[]+)\s*\[\s*(var\.|const\.)\s*(scal\.|vec\.|matr\.|tens\.|w\.f\.o\.)?\s*([A-Za-z0-9_]+)?\s*(,\s*[A-Za-z0-9\-\+\{\}\^_\/\s]+)?(,\s*[A-Za-z0-9\-\+\{\}\^_\/\s]+)?\s*\](.+)$";

        public bool IsLineIdentifierLine(string line)
        {
            return Regex.IsMatch(line, _identifierPattern);
        }

        public Identifier GetIdentifier(string line)
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
            else if (match.Groups[3].Value == "w.f.o.")
            {
                identifier.ObjectType = ObjectType.WaveFunctionObject;
            }

            identifier.Reference = match.Groups[4].Value.Trim();

            if (match.Groups[5].Value.Trim() != "")
            {
                identifier.Dimensions = "\\mathrm{" + match.Groups[5].Value.Trim().Substring(1).Trim() + "}";
            }

            if (match.Groups[6].Value.Trim() != "")
            {
                identifier.Units = "\\mathrm{" + match.Groups[6].Value.Trim().Substring(1).Trim() + "}";
            }

            identifier.Definition = match.Groups[7].Value.Trim();

            return identifier;
        }

        public Formula CompileFormula(string[] lines, IEnumerable<References.Reference> references)
        {
            lines = RemoveEmptyLines(lines);

            var formula = new Formula();

            formula.Reference = lines[0].Trim();
            formula.Title = CorrectApostrophes(lines[1].Trim());
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

                if (line == "derivation:")
                {
                    formulaSection = FormulaSection.Derivation;
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

                if (line == "curricula:")
                {
                    formulaSection = FormulaSection.Curricula;
                    continue;
                }

                if (line.StartsWith("rating:"))
                {
                    formulaSection = FormulaSection.Rating;
                    continue;
                }

                if (line.Trim() == "exclude")
                {
                    formula.ExcludeFromFormulaOfTheDay = true;
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

                if (formulaSection == FormulaSection.Derivation)
                {
                    formula.Derivation += line + "\n";
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
                    if (IsLineBookCitationLine(line))
                    {
                        var citation = GetBookCitation(line);

                        var book = (references.First(r => r.CitationKey == citation.Item1).Copy()) as Book;

                        if (citation.Item2 != "")
                        {
                            book.PageNumber = int.Parse(citation.Item2);
                        }

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

                if (formulaSection == FormulaSection.Curricula)
                {
                    formula.Curricula.Add(line);
                    continue;
                }
            }

            formula.URLReference = _referenceConverter.GetURLReference(formula.Reference);
            _autotagger.Autotag(formula);

            return formula;
        }
    }
}
