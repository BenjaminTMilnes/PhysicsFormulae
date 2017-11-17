using System;
using System.Collections.Generic;
using System.IO;
using PhysicsFormulae.Compiler;
using Newtonsoft.Json;

namespace PhysicsFormulae.TerminalApplication
{
    public class Program
    {
        static void Main(string[] args)
        {
            var compiler = new Compiler.Compiler();
            var formulae = new List<Formula>();

            var directoryInfo = new DirectoryInfo(@"..\..\..\PhysicsFormulae.Formulae");
            var files = directoryInfo.GetFiles("*.formula");

            foreach (var file in files)
            {
                var lines = File.ReadAllLines(file.FullName);
                var formula = compiler.CompileFormula(lines);
                formulae.Add(formula);

                Console.WriteLine(formula.Reference);
                Console.WriteLine(formula.Identifiers.Count);
            }

            var serializer = new JsonSerializer();

            using (var streamWriter = new StreamWriter(@"..\..\..\PhysicsFormulae.Formulae\Compiled.json"))
            using (var jsonTextWriter = new JsonTextWriter(streamWriter))
            {
                serializer.Serialize(jsonTextWriter, formulae);
            }

            //  Console.ReadLine();
        }
    }
}
