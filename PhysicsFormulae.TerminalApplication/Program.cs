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

            var outputLocations = new List<string>() { @"..\..\..\PhysicsFormulae.Formulae\Compiled.json", @"..\..\..\PhysicsFormulae.WebApplication\formulae.json" };

            var serializer = new JsonSerializer();

            foreach (var outputLocation in outputLocations)
            {
                using (var streamWriter = new StreamWriter(outputLocation))
                using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(jsonTextWriter, formulae);
                }
            }

            //  Console.ReadLine();
        }
    }
}
