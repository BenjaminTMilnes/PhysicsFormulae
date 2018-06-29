using System.Collections.Generic;

namespace PhysicsFormulae.Compiler
{
    public class Constant
    {
        public string Reference { get; set; }
        public string Title { get; set; }
        public string Interpretation { get; set; }
        public ConstantType Type { get; set; }
        public string Symbol { get; set; }
        public IList<Value> Values { get; set; }
        public IList<Reference> References { get; set; }
        public IList<SeeMoreLink> SeeMore { get; set; }
        public IList<string> Tags { get; set; }

        public Constant()
        {
            Values = new List<Value>();
            References = new List<Reference>();
            SeeMore = new List<SeeMoreLink>();
            Tags = new List<string>();
        }
    }
}
