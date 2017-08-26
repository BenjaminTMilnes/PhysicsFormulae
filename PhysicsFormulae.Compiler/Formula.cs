using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsFormulae.Compiler
{
    public class Formula
    {
        public string Reference { get; set; }
        public string Title { get; set; }
        public string Interpretation { get; set; }
        public string Content { get; set; }
        public IList<Identifier> Identifiers { get; set; }

        public Formula()
        {
            Identifiers = new List<Identifier>();
        }
    }
}
