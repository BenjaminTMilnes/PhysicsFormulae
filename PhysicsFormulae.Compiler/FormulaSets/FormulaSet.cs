using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhysicsFormulae.Compiler.Formulae;

namespace PhysicsFormulae.Compiler.FormulaSets
{
    public class FormulaSet
    {
        public string Reference { get; set; }
        public string URLReference { get; set; }
        public string Title { get; set; }
        public IList<FormulaSetFormula> Formulae { get; set; }
        public IList<Identifier> Identifiers { get; set; }
        public IList<string> Fields { get; set; }
        public IList<SeeMoreLink> SeeMore { get; set; }
        public IList<string> Tags { get; set; }

        public FormulaSet()
        {
            Formulae = new List<FormulaSetFormula>();
            Identifiers = new List<Identifier>();
            Fields = new List<string>();
            SeeMore = new List<SeeMoreLink>();
            Tags = new List<string>();
        }
    }
}
