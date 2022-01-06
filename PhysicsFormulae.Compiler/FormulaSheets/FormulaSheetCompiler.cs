using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsFormulae.Compiler.FormulaSheets
{
    public class FormulaSheetCompiler : Compiler
    {
        public FormulaSheetCompiler(Autotagger autotagger) : base(autotagger)
        {

        }

        public FormulaSheet CompileFormulaSheet(string[] lines, IEnumerable<Formulae.Formula> formulae)
        {
            lines = RemoveEmptyLines(lines);

            var formulaSheet = new FormulaSheet();

            formulaSheet.Reference = lines[0].Trim();
            formulaSheet.URLReference = _referenceConverter.GetURLReference(formulaSheet.Reference);
            formulaSheet.Title = CorrectApostrophes(lines[1].Trim());

            for (var i = 2; i < lines.Length; i++)
            {
                var r = lines[i].Trim();

                if (formulae.Any(g => g.Reference == r))
                {
                    var f1 = formulae.First(g => g.Reference == r);

                    var f2 = new FormulaSheetFormula();

                    f2.Reference = f1.Reference;
                    f2.URLReference = f1.URLReference;
                    f2.Title = f1.Title;
                    f2.Content = f2.Content;

                    formulaSheet.Formulae.Add(f2);
                }
            }

            return formulaSheet;
        }
    }
}
