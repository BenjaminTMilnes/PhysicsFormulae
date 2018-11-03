using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PhysicsFormulae.Compiler.References;

namespace PhysicsFormulae.Compiler.Constants
{
    public class Constant
    {
        public string Reference { get; set; }
        public string URLReference { get; set; }
        public string Title { get; set; }
        public string Interpretation { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ConstantType Type { get; set; }

        public string Symbol { get; set; }
        public IList<Value> Values { get; set; }
        public IList<Reference> References { get; set; }
        public IList<SeeMoreLink> SeeMore { get; set; }
        public IList<string> Tags { get; set; }

         public IList<string> UsedInFormulae { get; set; }

        public Constant()
        {
            Values = new List<Value>();
            References = new List<Reference>();
            SeeMore = new List<SeeMoreLink>();
            Tags = new List<string>();
            UsedInFormulae = new List<string>();
        }
    }
}
