using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhysicsFormulae.Compiler.Formulae;

namespace PhysicsFormulae.Compiler.Tests
{
    [TestClass]
    public class FormulaCompilerTests
    {
        protected Autotagger _autotagger;
        protected FormulaCompiler _formulaCompiler;

        public FormulaCompilerTests()
        {
            _autotagger = new Autotagger(new List<string>(), new List<string>());
            _formulaCompiler = new FormulaCompiler(_autotagger);
        }

        [TestMethod]
        public void TestGetIdentifier1()
        {
            var line = "F [var. scal. Force, M L T^{-2}, N] is the magnitude of the force experienced by each point charge";

            Assert.IsTrue(_formulaCompiler.IsLineIdentifierLine(line));

            var identifier = _formulaCompiler.GetIdentifier(line);

            Assert.AreEqual("F", identifier.Content);
            Assert.AreEqual(IdentifierType.Variable, identifier.Type);
            Assert.AreEqual(ObjectType.Scalar, identifier.ObjectType);
            Assert.AreEqual("Force", identifier.Reference);
            Assert.AreEqual("M L T^{-2}", identifier.Dimensions);
            Assert.AreEqual("N", identifier.Units);
            Assert.AreEqual("is the magnitude of the force experienced by each point charge", identifier.Definition);
        }
    }
}
