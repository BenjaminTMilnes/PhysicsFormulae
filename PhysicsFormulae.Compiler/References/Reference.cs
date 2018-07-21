using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PhysicsFormulae.Compiler.References
{
    public abstract class Reference
    {
        public string CitationKey { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ReferenceType Type { get; protected set; }
    }
}
