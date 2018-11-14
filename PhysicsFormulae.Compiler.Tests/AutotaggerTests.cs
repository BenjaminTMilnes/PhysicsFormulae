using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhysicsFormulae.Compiler;

namespace PhysicsFormulae.Compiler.Tests
{
    [TestClass]
    public class AutotaggerTests
    {
        protected Autotagger _autotagger { get; set; }

        public AutotaggerTests()
        {
            _autotagger = new Autotagger(new List<string>(), new List<string>());
        }

        [TestMethod]
        public void TestRemoveLaTeX1()
        {
            Assert.AreEqual("where  is the distance", _autotagger.RemoveLaTeX("where $x$ is the distance"));
        }

        [TestMethod]
        public void TestRemoveLaTeX2()
        {
            Assert.AreEqual("where  is the distance,  is the volume,  is the temperature", _autotagger.RemoveLaTeX("where $x$ is the distance, $V$ is the volume, $T$ is the temperature"));
        }

        [TestMethod]
        public void TestRemoveLaTeX3()
        {
            Assert.AreEqual("where ", _autotagger.RemoveLaTeX("where $x = 2y - 4$"));
        }
    }
}
