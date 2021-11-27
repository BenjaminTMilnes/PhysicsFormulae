using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsFormulae.Compiler.Formulae
{
    public class Curriculum
    {
        public string Name { get; set; }
        public IList<string> Formulae { get; set; }
        public IList<string> Constants { get; set; }

        public Curriculum()
        {
            Name = "";
            Formulae = new List<string>();
            Constants = new List<string>();
        }
    }
}
