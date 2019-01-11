using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PhysicsFormulae.Compiler;
using PhysicsFormulae.Compiler.Formulae;
using PhysicsFormulae.Compiler.Constants;
using PhysicsFormulae.Compiler.References;
using System.Xml;
using System.Xml.Linq;
using MathematicsTypesetting;
using MathematicsTypesetting.Fonts;
using MathematicsTypesetting.LaTeX;

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
            var directoryInfo = new DirectoryInfo(@"..\..\..\PhysicsFormulae.Formulae");
            var formulaFiles = directoryInfo.GetFiles("*.formula");
            var constantFiles = directoryInfo.GetFiles("*.constant");
            var referenceFiles = directoryInfo.GetFiles("*.reference");

            var excludedWordsFile = directoryInfo.GetFiles("ExcludedWords.txt").First();
            var keyPhrasesFile = directoryInfo.GetFiles("KeyPhrases.txt").First();

            var excludedWords = File.ReadAllLines(excludedWordsFile.FullName).Where(l => l != "").Select(l => l.Trim());
            var keyPhrases = File.ReadAllLines(keyPhrasesFile.FullName).Where(l => l != "").Select(l => l.Trim());

            var autotagger = new Autotagger(excludedWords, keyPhrases);

            var formulaCompiler = new FormulaCompiler(autotagger);
            var constantCompiler = new ConstantCompiler(autotagger);
            var referenceCompiler = new ReferenceCompiler(autotagger);

            var formulae = new List<Formula>();
            var constants = new List<Constant>();
            var references = new List<Reference>();

            foreach (var file in referenceFiles)
            {
                var lines = File.ReadAllLines(file.FullName);
                var reference = referenceCompiler.CompileReference(lines);
                references.Add(reference);

                Console.WriteLine(reference.CitationKey);
            }

            foreach (var file in formulaFiles)
            {
                var lines = File.ReadAllLines(file.FullName);
                var formula = formulaCompiler.CompileFormula(lines, references);
                formulae.Add(formula);

                Console.WriteLine(formula.Reference);
            }

            foreach (var file in constantFiles)
            {
                var lines = File.ReadAllLines(file.FullName);
                var constant = constantCompiler.CompileConstant(lines);
                constants.Add(constant);

                Console.WriteLine(constant.Reference);
            }

            foreach (var constant in constants)
            {
                foreach (var formula in formulae)
                {
                    if (formula.Identifiers.Any(i => i.Reference == constant.Reference))
                    {
                        constant.UsedInFormulae.Add(formula.Reference);
                    }
                }
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

            MakeFormulaImages(formulae);

            var sitemap = new Sitemap();

            foreach (var formula in formulae)
            {
                sitemap.AddURL("http://www.physicsformulae.com/#/formula/" + formula.URLReference);
            }

            foreach (var constant in constants)
            {
                sitemap.AddURL("http://www.physicsformulae.com/#/constant/" + constant.URLReference, "0.9");
            }

            sitemap.Save(@"..\..\..\PhysicsFormulae.WebApplication\sitemap.xml");
        }

        public static void MakeFormulaImages(IEnumerable<Formula> formulae)
        {
            var fontLoader = new FontLoader();
            var textMeasurer = new HyperfontTextMeasurer(fontLoader);
            var typesetter = new Typesetter(textMeasurer);
            var exporter = new PNGExporter(fontLoader);
            var parser = new LaTeXParser();

            foreach (var formula in formulae)
            {
                var document = new Document();

                document.MainElement = parser.ParseLaTeX(formula.Content);

                typesetter.TypesetDocument(document);

                var fileLocation = Path.Combine(@"..\..\..\PhysicsFormulae.WebApplication\images\", formula.URLReference + ".png");

                exporter.ExportMathematics(document, fileLocation);
            }
        }
    }
}
