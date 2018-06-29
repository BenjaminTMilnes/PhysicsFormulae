using System;
using System.Collections.Generic;
using System.IO;
using PhysicsFormulae.Compiler;
using Newtonsoft.Json;

namespace PhysicsFormulae.TerminalApplication
{
    public class Model
    {
        public IEnumerable<Formula> Formulae { get; set; }
        public IEnumerable<Constant> Constants { get; set; }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            var compiler = new Compiler.Compiler();
            var formulae = new List<Formula>();
            var constants = new List<Constant>();
            var references = new List<Reference>();

            var directoryInfo = new DirectoryInfo(@"..\..\..\PhysicsFormulae.Formulae");
            var files = directoryInfo.GetFiles("*.formula");
            var constantFiles = directoryInfo.GetFiles("*.constant");
            var referenceFiles = directoryInfo.GetFiles("*.reference");

            foreach (var file in files)
            {
                var lines = File.ReadAllLines(file.FullName);
                var formula = compiler.CompileFormula(lines);
                formulae.Add(formula);

                Console.WriteLine(formula.Reference);
                Console.WriteLine(formula.Identifiers.Count);
            }

            foreach (var file in constantFiles)
            {
                var lines = File.ReadAllLines(file.FullName);
                var constant = compiler.CompileConstant(lines);
                constants.Add(constant);

                Console.WriteLine(constant.Reference);
            }

            foreach (var file in referenceFiles)
            {
                var lines = File.ReadAllLines(file.FullName);
                var reference = compiler.CompileReference(lines);
                references.Add(reference);

                Console.WriteLine(reference.CitationKey);
            }

            var model = new Model();

            model.Formulae = formulae;
            model.Constants = constants;

            var outputLocations = new List<string>() { @"..\..\..\PhysicsFormulae.Formulae\Compiled.json", @"..\..\..\PhysicsFormulae.WebApplication\formulae.json" };

            var serializer = new JsonSerializer();

            foreach (var outputLocation in outputLocations)
            {
                using (var streamWriter = new StreamWriter(outputLocation))
                using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(jsonTextWriter, model);
                }
            }

            //  Console.ReadLine();
        }
    }
}
