using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsFormulae.Compiler.FormulaSets
{
    public class FormulaSetCompiler : Compiler
    {
        public FormulaSetCompiler(Autotagger autotagger) : base(autotagger)
        {

        }

        public FormulaSet CompileFormulaSet(string[] lines, IEnumerable<Formulae.Formula> formulae)
        {
            lines = RemoveEmptyLines(lines);

            var formulaSet = new FormulaSet();

            formulaSet.Reference = lines[0].Trim();
            formulaSet.URLReference = _referenceConverter.GetURLReference(formulaSet.Reference);
            formulaSet.Title = CorrectApostrophes(lines[1].Trim());

            for (var i = 2; i < lines.Length; i++)
            {
                var r = lines[i].Trim();

                if (formulae.Any(g => g.Reference == r))
                {
                    var f1 = formulae.First(g => g.Reference == r);

                    var f2 = new FormulaSetFormula();

                    f2.Reference = f1.Reference;
                    f2.URLReference = f1.URLReference;
                    f2.Title = f1.Title;
                    f2.Interpretation = f1.Interpretation;
                    f2.Content = f1.Content;

                    formulaSet.Formulae.Add(f2);

                    formulaSet.Identifiers = formulaSet.Identifiers.Concat(f1.Identifiers).ToList();
                    formulaSet.Fields = formulaSet.Fields.Concat(f1.Fields).ToList();
                    formulaSet.Tags = formulaSet.Tags.Concat(f1.Tags).ToList();
                }
            }

            formulaSet.Identifiers = formulaSet.Identifiers.Distinct().ToList();
            formulaSet.Fields = formulaSet.Fields.Distinct().ToList();
            formulaSet.Tags = formulaSet.Tags.Distinct().ToList();

            return formulaSet;
        }
    }
}
