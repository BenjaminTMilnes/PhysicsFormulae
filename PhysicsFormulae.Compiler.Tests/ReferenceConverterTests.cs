using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhysicsFormulae.Compiler.Tests
{
    [TestClass]
    public class ReferenceConverterTests
    {
        protected ReferenceConverter _referenceConverter;

        public ReferenceConverterTests()
        {
            _referenceConverter = new ReferenceConverter();
        }

        [TestMethod]
        public void TestGetURLReference1()
        {
            Assert.AreEqual("definition-of-pressure", _referenceConverter.GetURLReference("DefinitionOfPressure"));
        }

        [TestMethod]
        public void TestGetURLReference2()
        {
            Assert.AreEqual("suvat1", _referenceConverter.GetURLReference("SUVAT1"));
        }
    }
}
