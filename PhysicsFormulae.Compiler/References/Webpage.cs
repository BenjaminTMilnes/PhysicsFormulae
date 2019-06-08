using System;

namespace PhysicsFormulae.Compiler.References
{
    public class Webpage : Reference
    {
        public string Title { get; set; }
        public string WebsiteTitle { get; set; }
        public string URL { get; set; }
        public DateTime DateAccessed { get; set; }

        public Webpage()
        {
            CitationKey = "";
            Type = ReferenceType.Webpage;
        }

        public override Reference Copy()
        {
            throw new NotImplementedException();
        }
    }
}
