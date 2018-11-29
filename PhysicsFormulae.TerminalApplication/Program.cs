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

            var xmlNameSpace = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");
            var sitemap = new XDocument(new XDeclaration("1.0", "UTF-8", null));
            
            sitemap.Add(new XElement(xmlNameSpace + "urlset"));

            var root = sitemap.Root;

            foreach (var formula in formulae)
            {
                var url = new XElement(xmlNameSpace + "url");

                var location = new XElement(xmlNameSpace + "loc");
                location.Add(new XText( "http://www.physicsformulae.com/#/formula/" + formula.URLReference));

                var lastModifiedDate = new XElement(xmlNameSpace + "lastmod");
                lastModifiedDate.Add(new XText(DateTime.UtcNow.ToString()));

                var priority = new XElement(xmlNameSpace + "priority");
                priority.Add(new XText("1.0"));

                url.Add(location);
                url.Add(lastModifiedDate);
                url.Add(priority);

                root.Add(url);
            }

            foreach (var constant in constants)
            {
                var url = new XElement(xmlNameSpace + "url");

                var location = new XElement(xmlNameSpace + "loc");
                location.Add(new XText("http://www.physicsformulae.com/#/constant/" + constant.URLReference));

                var lastModifiedDate = new XElement(xmlNameSpace + "lastmod");
                lastModifiedDate.Add(new XText(DateTime.UtcNow.ToString()));

                var priority = new XElement(xmlNameSpace + "priority");
                priority.Add(new XText("0.9"));

                url.Add(location);
                url.Add(lastModifiedDate);
                url.Add(priority);

                root.Add(url);
            }

            sitemap.Save(@"..\..\..\PhysicsFormulae.WebApplication\sitemap.xml");
        }
    }
}
