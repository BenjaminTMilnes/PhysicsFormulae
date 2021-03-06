﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PhysicsFormulae.Compiler.Formulae
{
    public class Identifier
    {
        public string Content { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public IdentifierType Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ObjectType ObjectType { get; set; }

        public string Reference { get; set; }

        public string Dimensions { get; set; }

        public string Units { get; set; }

        public string Definition { get; set; }
    }
}
