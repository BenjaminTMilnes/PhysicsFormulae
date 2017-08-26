using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsFormulae.Compiler
{
    public class Compiler
    {
        public Formula CompileFormula(string[] lines)
        {
            var formula = new Formula();

            formula.Reference = lines[0].Trim();
            formula.Title = lines[1].Trim();
            formula.Interpretation = lines[2].Trim();
            formula.Content = lines[3].Trim();

            return formula;
        }
    }
}
