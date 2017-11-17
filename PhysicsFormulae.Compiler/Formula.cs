using System.Collections.Generic;

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
