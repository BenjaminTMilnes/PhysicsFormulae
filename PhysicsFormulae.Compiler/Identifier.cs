using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PhysicsFormulae.Compiler
{
    public class Identifier
    {
        public string Content { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public IdentifierType Type { get; set; }

        public string Reference { get; set; }

        public string Definition { get; set; }
    }
}
