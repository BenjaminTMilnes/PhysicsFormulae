using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhysicsFormulae.Compiler.Formulae;
using System.Collections.Generic;

namespace PhysicsFormulae.Compiler.Tests
{
    [TestClass]
    public class CompilerTests
    {
        protected Autotagger _autotagger;
        protected FormulaCompiler _formulaCompiler;

        public CompilerTests()
        {
            _autotagger = new Autotagger(new List<string>(), new List<string>());
            _formulaCompiler = new FormulaCompiler(_autotagger);
        }

        [TestMethod]
        public void TestIsSeeMoreLinkLine1()
        {
            Assert.AreEqual(true, _formulaCompiler.IsSeeMoreLinkLine("Wikipedia http://www.wikipedia.org"));
        }

        [TestMethod]
        public void TestIsSeeMoreLinkLine2()
        {
            Assert.AreEqual(false, _formulaCompiler.IsSeeMoreLinkLine("Wikipedia"));
        }

        [TestMethod]
        public void TestGetSeeMoreLink1()
        {
            Assert.AreEqual("Wikipedia", _formulaCompiler.GetSeeMoreLink("Wikipedia http://www.wikipedia.org").Name);
        }

        [TestMethod]
        public void TestGetSeeMoreLink2()
        {
            Assert.AreEqual("http://www.wikipedia.org", _formulaCompiler.GetSeeMoreLink("Wikipedia http://www.wikipedia.org").URL);
        }
    }
}
