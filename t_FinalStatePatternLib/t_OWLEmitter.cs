using FinalStatePatternLib;
using FinalStatePatternLib.OWLData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

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
            Assert.AreEqual("<#hi> rdf:type dfs:PhysicsObject ;\r\n  hasBaseDefinition: \"fork\" .", text);
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

        [TestMethod]
        public void EmitSelectionCriteria()
        {
            var n1 = new PhysicalValue() { Number = 51 };
            var n2 = new PhysicalValue() { Number = 52 };
            var sc = new SelectionCriteria()
            {
                BinaryRelation = ">",
                FirstArgument = n1,
                SecondArgument = n2
            };
            var ms = new StringWriter();
            sc.Emit(ms);
            var text = ms.ToString().Split('\r').Select(l => l.Trim()).ToArray();
            Assert.AreEqual("<#number0> rdf:type dfs:NumericalValue ;", text[0]);
            Assert.AreEqual("dfs:hasNumber \"51\"^^xsd:decimal .", text[1]);
            Assert.AreEqual("", text[2]);
            Assert.AreEqual("<#number1> rdf:type dfs:NumericalValue ;", text[3]);
            Assert.AreEqual("dfs:hasNumber \"52\"^^xsd:decimal .", text[4]);
            Assert.AreEqual("", text[5]);

            Assert.AreEqual("<#selectionCriteria2> rdf:type dfs:SelectionCriteria ;", text[6]);
            Assert.AreEqual("dfs:usesBinaryRelation dfs:greaterThan ;", text[7]);
            Assert.AreEqual("dfs:hasFirstArgument <#number0> ;", text[8]);
            Assert.AreEqual("dfs:hasSecondArgument <#number1> .", text[9]);
        }
    }
}
