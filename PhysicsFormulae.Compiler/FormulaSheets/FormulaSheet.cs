using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsFormulae.Compiler.FormulaSheets
{
    public class FormulaSheet
    {
        public string Reference { get; set; }
        public string URLReference { get; set; }
        public string Title { get; set; }
        public IList<FormulaSheetFormula> Formulae { get; set; }

        public FormulaSheet()
        {
            Formulae = new List<FormulaSheetFormula>();
        }
    }
}
