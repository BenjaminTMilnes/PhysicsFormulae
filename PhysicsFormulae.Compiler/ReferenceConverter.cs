using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PhysicsFormulae.Compiler
{
    public class ReferenceConverter
    {
        public string GetURLReference(string reference)
        {
            var urlReference = reference;

            urlReference = Regex.Replace(urlReference, @"(.+?)([A-Z0-9])", @"$1-$2");
            urlReference = urlReference.ToLower();

            return urlReference;
        }
    }
}
