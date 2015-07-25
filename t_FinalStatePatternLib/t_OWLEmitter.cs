using FinalStatePatternLib;
using FinalStatePatternLib.OWLData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace t_FinalStatePatternLib
{
    [TestClass]
    public class t_OWLEmitter
    {
        [TestMethod]
        public void EmitAFSOWithNoBaseDefinition()
        {
            var fso = new FinalStateObject() { Name = "hi", BaseDefinition = null };
            var ms = new StringWriter();
            fso.Emit(ms);
            var text = ms.ToString().Trim();
            Assert.AreEqual("<#hi> rdf:type dfs:PhysicsObject .", text);
        }

        [TestMethod]
        public void EmitAFSOWithBaseDefinition()
        {
            var fso = new FinalStateObject() { Name = "hi", BaseDefinition = "fork" };
            var ms = new StringWriter();
            fso.Emit(ms);
            var text = ms.ToString().Trim();
            Assert.AreEqual("<#hi> rdf:type dfs:PhysicsObject; \n  hasBaseDefinition: fork .", text);
        }
    }
}
