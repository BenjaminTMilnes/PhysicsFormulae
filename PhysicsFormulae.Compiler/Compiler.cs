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
        References = 5
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

                if (line.Trim() == "where:")
                {
                    formulaSection = FormulaSection.Where;
                    continue;
                }

                if (line.Trim() == "derived from:")
                {
                    formulaSection = FormulaSection.DerivedFrom;
                    continue;
                }

                if (line.Trim() == "fields:")
                {
                    formulaSection = FormulaSection.Fields;
                    continue;
                }

                if (line.Trim() == "references:")
                {
                    formulaSection = FormulaSection.References;
                    continue;
                }

                if (formulaSection == FormulaSection.Where)
                {
                    if (IsLineIdentifierLine(line))
                    {
                        var identifier = GetIdentifier(line);

                        formula.Identifiers.Add(identifier);
                    }
                }
            }

            return formula;
        }

        private string[] RemoveEmptyLines(string[] lines)
        {
            return lines.Where(l => l.Trim() != "").ToArray();
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
    }
}
