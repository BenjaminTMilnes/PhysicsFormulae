using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsFormulae.Compiler
{
    public class Identifier
    {
        public string Content { get; set; }
        public IdentifierType Type { get; set; }
        public string Reference { get; set; }
        public string Definition { get; set; }
    }
}
