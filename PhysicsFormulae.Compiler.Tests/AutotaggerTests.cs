using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PhysicsFormulae.Compiler;

namespace PhysicsFormulae.Compiler.Tests
{
    [TestFixture]
    public class AutotaggerTests
    {
        protected Autotagger _autotagger { get; set; }

        public AutotaggerTests()
        {
            _autotagger = new Autotagger();
        }

        [TestCase("where  is the distance", "where $x$ is the distance")]
        public void TestRemoveLaTeX(string normalised, string unnormalised)
        {
            Assert.AreEqual(normalised, _autotagger.RemoveLaTeX(unnormalised));
        }
    }
}
