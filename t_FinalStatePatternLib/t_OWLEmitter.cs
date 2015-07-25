using FinalStatePatternLib;
using FinalStatePatternLib.OWLData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace t_FinalStatePatternLib
{
    [TestClass]
    public class t_OWLEmitter
    {
        [TestInitialize]
        public void Setup()
        {
            OWLEmitter.Reset();
        }

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

        [TestMethod]
        public void EmitPhysicalNumberNoUnit()
        {
            var pn = new PhysicalValue() { Number = 55.0 };
            var ms = new StringWriter();
            pn.Emit(ms);
            var text = ms.ToString().Trim();
            Assert.AreEqual("<#number0> rdf:type dfs:NumericalValue ;\r\n  dfs:hasNumber \"55\"^^xsd:decimal .", text);
        }

        [TestMethod]
        public void EmitPhysicalNumberUnit()
        {
            var pn = new PhysicalValue() { Number = 55.0, Unit = "GeV" };
            var ms = new StringWriter();
            pn.Emit(ms);
            var text = ms.ToString().Trim();
            Assert.AreEqual("<#number0> rdf:type dfs:NumericalValue ;\r\n  dfs:hasNumber \"55\"^^xsd:decimal ;\r\n  dfs:hasUnit dfs:GeV .", text);
        }

        [TestMethod]
        public void EmitSinglePhysicalValue()
        {
            var fso = new FinalStateObject() { Name = "J1", BaseDefinition = null };
            var spq = new SinglePhysicalQuantity() { RefersToObject = fso, PhysicalQantity = "pT" };
            var ms = new StringWriter();
            spq.Emit(ms);
            var text = ms.ToString().Trim();
            Assert.AreEqual("<#physicalQuantity0> rdf:type dfs:PhysicalQuantity ;\r\n  dfs:refersToObject <#J1> ;\r\n  dfs:hasQuantity dfs:pT .", text);
        }

        [TestMethod]
        public void EmitFunction()
        {
            var fso1 = new FinalStateObject() { Name = "J1", BaseDefinition = null };
            var fso2 = new FinalStateObject() { Name = "J2", BaseDefinition = null };
            var fpv = new FunctionPhysicalQuantity() { ArgumentList = "J1.pT > 20, ET > 0, J2", Name = "NTrack", RefersToObjects = new FinalStateObject[] { fso1, fso2 } };
            var ms = new StringWriter();
            fpv.Emit(ms);
            var text = ms.ToString().Trim();
            Assert.AreEqual("<#functionQuantity0> rdf:type dfs:PhysicalQuantity ;\r\n  dfs:refersToObject <#J1> ;\r\n  dfs:refersToObject <#J2> ;\r\n  dfs:hasQuantity \"NTrack(J1.pT > 20, ET > 0, J2)\" .", text);
        }
    }
}
