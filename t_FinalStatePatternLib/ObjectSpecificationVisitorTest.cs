using FinalStatePatternLib;
using FinalStatePatternLib.OWLData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace t_FinalStatePatternLib
{
    [TestClass]
    public class ObjectSpecificationVisitorTest
    {
        /// <summary>
        /// Test a name object with no restrictions (e.g. just a definition).
        /// </summary>
        [TestMethod]
        public void SingleName()
        {
            var text = "J1;";
            var dfs = text.Parse();
            Assert.AreEqual(1, dfs.FinalStateObjects.Count);
            Assert.AreEqual("J1", dfs.FinalStateObjects[0].Name);
            Assert.IsNull(dfs.FinalStateObjects[0].BaseDefinition);
        }

        [TestMethod]
        public void NameWithBase()
        {
            var text = "J1 (atlas-anti-kt4);";
            var dfs = text.Parse();
            Assert.AreEqual(1, dfs.FinalStateObjects.Count);
            Assert.AreEqual("J1", dfs.FinalStateObjects[0].Name);
            Assert.AreEqual("atlas-anti-kt4", dfs.FinalStateObjects[0].BaseDefinition);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void NameWithTwoBase()
        {
            var text = "J1(atlas1); J1(atlas2);";
            var dfs = text.Parse();
        }

        [TestMethod]
        public void SingleCut()
        {
            var text = "J1: pT>30 GeV;";
            var dfs = text.Parse();

            Assert.AreEqual(FinalStatePatternLib.OWLData.ANDORType.kAnd, dfs.Criteria.AOType);
            Assert.AreEqual(1, dfs.Criteria.Arguments.Count);
            Assert.IsInstanceOfType(dfs.Criteria.Arguments[0], typeof(SelectionCriteria));
            var a1 = dfs.Criteria.Arguments[0] as SelectionCriteria;
            Assert.AreEqual(">", a1.BinaryRelation);
            Assert.IsInstanceOfType(a1.FirstArgument, typeof(SinglePhysicalQuantity));
            Assert.IsInstanceOfType(a1.SecondArgument, typeof(PhysicalValue));

            var spq = a1.FirstArgument as SinglePhysicalQuantity;
            var pv = a1.SecondArgument as PhysicalValue;

            Assert.AreEqual(30.0, pv.Number);
            Assert.AreEqual("GeV", pv.Unit);

            Assert.AreEqual("J1", spq.RefersToObject);
            Assert.AreEqual("pT", spq.PhysicalQantity);
        }

        [TestMethod]
        public void SingleCutWithDecimal()
        {
            var text = "J1: absEta> 2.5;";
            var dfs = text.Parse();

            var a1 = dfs.Criteria.Arguments[0] as SelectionCriteria;
            var pv = a1.SecondArgument as PhysicalValue;
            Assert.AreEqual(2.5, pv.Number);
        }

        [TestMethod]
        public void TwoCuts()
        {
            var text = "J1: pT>30 GeV, absEta<2.5;";
            var dfs = text.Parse();

            Assert.AreEqual(2, dfs.Criteria.Arguments.Count);

            var a1 = dfs.Criteria.Arguments[0] as SelectionCriteria;
            var a2 = dfs.Criteria.Arguments[1] as SelectionCriteria;

            var spq1 = a1.FirstArgument as SinglePhysicalQuantity;
            var spq2 = a2.FirstArgument as SinglePhysicalQuantity;

            Assert.AreEqual("J1", spq1.RefersToObject);
            Assert.AreEqual("J1", spq2.RefersToObject);
            Assert.AreEqual("pT", spq1.PhysicalQantity);
            Assert.AreEqual("absEta", spq2.PhysicalQantity);
        }

        [TestMethod]
        public void RangeCut()
        {
            var text = "J1: 20 GeV < pT < 30 GeV;";
            var dfs = text.Parse();

            Assert.AreEqual(2, dfs.Criteria.Arguments.Count);

            var a1 = dfs.Criteria.Arguments[0] as SelectionCriteria;
            var a2 = dfs.Criteria.Arguments[1] as SelectionCriteria;

            var spq1 = a1.SecondArgument as SinglePhysicalQuantity;
            var spq2 = a2.FirstArgument as SinglePhysicalQuantity;

            Assert.AreEqual("J1", spq1.RefersToObject);
            Assert.AreEqual("J1", spq2.RefersToObject);
            Assert.AreEqual("pT", spq1.PhysicalQantity);
            Assert.AreEqual("pT", spq2.PhysicalQantity);

            var pv1 = a1.FirstArgument as PhysicalValue;
            var pv2 = a2.SecondArgument as PhysicalValue;

            Assert.AreEqual(20.0, pv1.Number);
            Assert.AreEqual(30.0, pv2.Number);
            Assert.AreEqual("GeV", pv1.Unit);
            Assert.AreEqual("GeV", pv2.Unit);
        }

        [TestMethod]
        public void SingleCutWithDefinedName()
        {
            var text = "J1(atlas-anti-kt4): pT > 50 GeV; J1.pT < 100 GeV;";
            var dfs = text.Parse();

            Assert.AreEqual(2, dfs.Criteria.Arguments.Count);

            Assert.IsInstanceOfType(dfs.Criteria.Arguments[1], typeof(SelectionCriteria));
            var a2 = dfs.Criteria.Arguments[1] as SelectionCriteria;

            Assert.IsInstanceOfType(a2.FirstArgument, typeof(SinglePhysicalQuantity));
            Assert.IsInstanceOfType(a2.SecondArgument, typeof(PhysicalValue));
            var spq = a2.FirstArgument as SinglePhysicalQuantity;
            var pv = a2.SecondArgument as PhysicalValue;

            Assert.AreEqual("<", a2.BinaryRelation);
            Assert.AreEqual("J1", spq.RefersToObject);
            Assert.AreEqual("pT", spq.PhysicalQantity);
            Assert.AreEqual(100.0, pv.Number);
            Assert.AreEqual("GeV", pv.Unit);
        }

        [TestMethod]
        public void RangeCutWithDefinedName()
        {
            var text = "J1(atlas-anti-kt4): pT > 50 GeV; 60 GeV < J1.pT < 100 GeV;";
            var dfs = text.Parse();

            Assert.Inconclusive();
        }

        [TestMethod]
        public void NameInSingleCutList()
        {
            var text = "ETMiss(atlas-met) < 50 GeV;";
            var dfs = text.Parse();

            Assert.AreEqual(1, dfs.FinalStateObjects.Count);
            Assert.AreEqual(1, dfs.Criteria.Arguments.Count);

            Assert.AreEqual("ETMiss", dfs.FinalStateObjects[0].Name);
            Assert.AreEqual("atlas-met", dfs.FinalStateObjects[0].BaseDefinition);

            Assert.AreEqual("<", (dfs.Criteria.Arguments[0] as SelectionCriteria).BinaryRelation);
            Assert.AreEqual("ETMiss", ((dfs.Criteria.Arguments[0] as SelectionCriteria).FirstArgument as SinglePhysicalQuantity).PhysicalQantity);
        }
    }
}
