using System.Collections.Generic;
using PhysicsFormulae.Compiler.References;

namespace PhysicsFormulae.Compiler.Formulae
{
    public class Formula
    {
        public string Reference { get; set; }
        public string URLReference { get; set; }
        public string Title { get; set; }
        public string Interpretation { get; set; }
        public string Content { get; set; }
        public IList<Identifier> Identifiers { get; set; }
        public IList<Variant> Variants { get; set; }
        public IList<string> DerivedFrom { get; set; }
        public string Derivation { get; set; }
        public IList<string> Fields { get; set; }
        public IList<Reference> References { get; set; }
        public IList<SeeMoreLink> SeeMore { get; set; }
        public IList<string> Tags { get; set; }
        public bool ExcludeFromFormulaOfTheDay { get; set; }

        public Formula()
        {
            Identifiers = new List<Identifier>();
            Variants = new List<Variant>();
            DerivedFrom = new List<string>();
            Fields = new List<string>();
            References = new List<Reference>();
            SeeMore = new List<SeeMoreLink>();
            Tags = new List<string>();
        }
    }
}
