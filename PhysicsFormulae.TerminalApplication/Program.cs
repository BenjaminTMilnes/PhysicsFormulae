﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PhysicsFormulae.Compiler;
using PhysicsFormulae.Compiler.Formulae;
using PhysicsFormulae.Compiler.Constants;
using PhysicsFormulae.Compiler.References;
using PhysicsFormulae.Compiler.FormulaSets;
using PhysicsFormulae.Compiler.FormulaSheets;
using System.Xml;
using System.Xml.Linq;
//using MathematicsTypesetting;
//using MathematicsTypesetting.Fonts;
//using MathematicsTypesetting.LaTeX;

namespace PhysicsFormulae.TerminalApplication
{
    public class Model
    {
        public IEnumerable<Formula> Formulae { get; set; }
        public IEnumerable<Constant> Constants { get; set; }
        public IEnumerable<Reference> References { get; set; }
        public IEnumerable<FormulaSet> FormulaSets { get; set; }
        public IEnumerable<Curriculum> Curricula { get; set; }
        public IEnumerable<FormulaSheet> FormulaSheets { get; set; }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            var directoryInfo = new DirectoryInfo(@"..\..\..\PhysicsFormulae.Formulae");
            var formulaFiles = directoryInfo.GetFiles("*.formula");
            var constantFiles = directoryInfo.GetFiles("*.constant");
            var referenceFiles = directoryInfo.GetFiles("*.reference");
            var formulaSetFiles = directoryInfo.GetFiles("*.formulaset");
            var formulaSheetFiles = directoryInfo.GetFiles("*.formulasheet");

            var excludedWordsFile = directoryInfo.GetFiles("ExcludedWords.txt").First();
            var keyPhrasesFile = directoryInfo.GetFiles("KeyPhrases.txt").First();

            var excludedWords = File.ReadAllLines(excludedWordsFile.FullName).Where(l => l != "").Select(l => l.Trim());
            var keyPhrases = File.ReadAllLines(keyPhrasesFile.FullName).Where(l => l != "").Select(l => l.Trim());

            var autotagger = new Autotagger(excludedWords, keyPhrases);

            var formulaCompiler = new FormulaCompiler(autotagger);
            var constantCompiler = new ConstantCompiler(autotagger);
            var referenceCompiler = new ReferenceCompiler(autotagger);
            var formulaSetCompiler = new FormulaSetCompiler(autotagger);
            var formulaSheetCompiler = new FormulaSheetCompiler(autotagger);

            var formulae = new List<Formula>();
            var constants = new List<Constant>();
            var references = new List<Reference>();
            var formulaSets = new List<FormulaSet>();
            var formulaSheets = new List<FormulaSheet>();

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

            foreach (var file in formulaSetFiles)
            {
                var lines = File.ReadAllLines(file.FullName);
                var formulaSet = formulaSetCompiler.CompileFormulaSet(lines, formulae);
                formulaSets.Add(formulaSet);

                Console.WriteLine(formulaSet.Reference);
            }

            foreach (var file in formulaSheetFiles)
            {
                var lines = File.ReadAllLines(file.FullName);
                var formulaSheet = formulaSheetCompiler.CompileFormulaSheet(lines, formulae);
                formulaSheets.Add(formulaSheet);

                foreach (var formula in formulaSheet.Formulae)
                {
                    var f = formulae.First(a => a.Reference == formula.Reference);

                    f.FormulaSheets.Add(new FormulaSheetUsage() { Reference = formulaSheet.Reference, URLReference = formulaSheet.URLReference, Title = formulaSheet.Title });
                }

                Console.WriteLine(formulaSheet.Reference);
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

            var curricula = new Dictionary<string, Curriculum>();

            foreach (var formula in formulae)
            {
                foreach (var curriculum in formula.Curricula)
                {
                    if (!curricula.ContainsKey(curriculum))
                    {
                        curricula[curriculum] = new Curriculum();
                        curricula[curriculum].Name = curriculum;
                    }

                    curricula[curriculum].Formulae.Add(formula.Reference);
                }
            }

            var model = new Model();

            model.Formulae = formulae;
            model.Constants = constants;
            model.References = references;
            model.FormulaSets = formulaSets;
            model.FormulaSheets = formulaSheets;
            model.Curricula = curricula.Select(c => c.Value).ToList();

            var outputLocations = new List<string>() { @"..\..\..\PhysicsFormulae.Formulae\Compiled.json", @"..\..\..\PhysicsFormulae.WebApplication\formulae.json" };

            var serializer = new JsonSerializer();

            serializer.Formatting = Newtonsoft.Json.Formatting.Indented;

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

            foreach (var formulaSet in formulaSets)
            {
                sitemap.AddURL("http://www.physicsformulae.com/#/formula-set/" + formulaSet.URLReference);
            }

            foreach (var constant in constants)
            {
                sitemap.AddURL("http://www.physicsformulae.com/#/constant/" + constant.URLReference, "0.9");
            }

            sitemap.Save(@"..\..\..\PhysicsFormulae.WebApplication\sitemap.xml");
        }

        public static void MakeFormulaImages(IEnumerable<Formula> formulae)
        {
            //   var renderer = new MathematicsTypesetting.Rendering.Renderer();
            //
            //    foreach (var formula in formulae)
            //   {
            //       var fileLocation = Path.Combine(@"..\..\..\PhysicsFormulae.WebApplication\images\", formula.URLReference + ".png");

            //       renderer.RenderMathematics(formula.Content, fileLocation);
            //   }
        }
    }
}
