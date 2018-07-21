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
    public class ReferenceConverterTests
    {
        protected ReferenceConverter _referenceConverter { get; set; }

        public ReferenceConverterTests()
        {
            _referenceConverter = new ReferenceConverter();
        }

        [TestMethod]
        public void TestGetURLReference1()
        {
            Assert.AreEqual("definition-of-pressure", _referenceConverter.GetURLReference("DefinitionOfPressure"));
        }
    }
}
