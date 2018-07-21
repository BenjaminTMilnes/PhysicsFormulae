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
            var urlReference = "";
            var breakLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            for (var i = 0; i < reference.Length; i++)
            {
                var c = reference[i];

                if (i > 0 && breakLetters.Any(l => l == c) && !breakLetters.Any(l => l == reference[i - 1]))
                {
                    urlReference += "-";
                }

                urlReference += c;
            }

            urlReference = urlReference.ToLower();

            return urlReference;
        }
    }
}
