using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PhysicsFormulae.Compiler
{
    public abstract class Reference
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ReferenceType Type { get; protected set; }
    }
}
