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

            Assert.AreEqual("J1", spq.RefersToObject.Name);
            Assert.AreEqual("pT", spq.PhysicalQantity);
        }

        [TestMethod]
        public void SingleNegCut()
        {
            var text = "J1: pT>-30 GeV;";
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

            Assert.AreEqual(-30.0, pv.Number);
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
        public void SingleNegCutWithDecimal()
        {
            var text = "J1: absEta> -2.5;";
            var dfs = text.Parse();

            var a1 = dfs.Criteria.Arguments[0] as SelectionCriteria;
            var pv = a1.SecondArgument as PhysicalValue;
            Assert.AreEqual(-2.5, pv.Number);
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

            Assert.AreEqual("J1", spq1.RefersToObject.Name);
            Assert.AreEqual("J1", spq2.RefersToObject.Name);
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

            Assert.AreEqual("J1", spq1.RefersToObject.Name);
            Assert.AreEqual("J1", spq2.RefersToObject.Name);
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
            Assert.AreEqual("J1", spq.RefersToObject.Name);
            Assert.AreEqual("pT", spq.PhysicalQantity);
            Assert.AreEqual(100.0, pv.Number);
            Assert.AreEqual("GeV", pv.Unit);
        }

        [TestMethod]
        public void RangeCutWithDefinedName()
        {
            var text = "J1(atlas-anti-kt4): pT > 50 GeV; 60 GeV < J1.pT < 100 GeV;";
            var dfs = text.Parse();

            Assert.AreEqual(3, dfs.Criteria.Arguments.Count);
        }

        [TestMethod]
        public void NameInSingleCutList()
        {
            var text = "ETMiss(atlas-met).ET < 50 GeV;";
            var dfs = text.Parse();

            Assert.AreEqual(1, dfs.FinalStateObjects.Count);
            Assert.AreEqual(1, dfs.Criteria.Arguments.Count);

            Assert.AreEqual("ETMiss", dfs.FinalStateObjects[0].Name);
            Assert.AreEqual("atlas-met", dfs.FinalStateObjects[0].BaseDefinition);

            Assert.AreEqual("<", (dfs.Criteria.Arguments[0] as SelectionCriteria).BinaryRelation);
            Assert.AreEqual("ET", ((dfs.Criteria.Arguments[0] as SelectionCriteria).FirstArgument as SinglePhysicalQuantity).PhysicalQantity);
            Assert.AreEqual("ETMiss", ((dfs.Criteria.Arguments[0] as SelectionCriteria).FirstArgument as SinglePhysicalQuantity).RefersToObject.Name);
        }

        [TestMethod]
        public void NameInSingleCutListSingleObjectName()
        {
            var text = "ETMiss(atlas-met) < 50 GeV;";
            var dfs = text.Parse();

            Assert.AreEqual(1, dfs.FinalStateObjects.Count);
            Assert.AreEqual(1, dfs.Criteria.Arguments.Count);

            Assert.AreEqual("ETMiss", dfs.FinalStateObjects[0].Name);
            Assert.AreEqual("atlas-met", dfs.FinalStateObjects[0].BaseDefinition);

            Assert.AreEqual("<", (dfs.Criteria.Arguments[0] as SelectionCriteria).BinaryRelation);

            // TODO: May be we want some sort of built-in alias mechanism, so this becomes ET or something like that.
            Assert.AreEqual("", ((dfs.Criteria.Arguments[0] as SelectionCriteria).FirstArgument as SinglePhysicalQuantity).PhysicalQantity);
            Assert.AreEqual("ETMiss", ((dfs.Criteria.Arguments[0] as SelectionCriteria).FirstArgument as SinglePhysicalQuantity).RefersToObject.Name);
        }

        [TestMethod]
        public void SingleLineFunctionRestrictionEmpty()
        {
            var text = "J2(atlas-jet); NTrack() = 0;";
            var dfs = text.Parse();

            Assert.AreEqual(1, dfs.FinalStateObjects.Count);
            Assert.AreEqual(1, dfs.Criteria.Arguments.Count);

            var s1 = dfs.Criteria.Arguments[0] as SelectionCriteria;
            Assert.IsInstanceOfType(s1.FirstArgument, typeof(FunctionPhysicalQuantity));
            Assert.IsInstanceOfType(s1.SecondArgument, typeof(PhysicalValue));
            Assert.AreEqual("=", s1.BinaryRelation);

            var f1 = s1.FirstArgument as FunctionPhysicalQuantity;
            Assert.AreEqual("NTrack", f1.Name);
            Assert.AreEqual("", f1.ArgumentList);
            Assert.AreEqual(0, f1.RefersToObjects.Length);
        }

        [TestMethod]
        public void SingleLineFunctionRestrictionFunctionCutArg()
        {
            var text = "J2(atlas-jet); NTrack(pT>0) = 0;";
            var dfs = text.Parse();

            Assert.AreEqual(1, dfs.FinalStateObjects.Count);
            Assert.AreEqual(1, dfs.Criteria.Arguments.Count);

            var s1 = dfs.Criteria.Arguments[0] as SelectionCriteria;
            Assert.IsInstanceOfType(s1.FirstArgument, typeof(FunctionPhysicalQuantity));
            Assert.IsInstanceOfType(s1.SecondArgument, typeof(PhysicalValue));
            Assert.AreEqual("=", s1.BinaryRelation);

            var f1 = s1.FirstArgument as FunctionPhysicalQuantity;
            Assert.AreEqual("NTrack", f1.Name);
            Assert.AreEqual("pT>0", f1.ArgumentList);
            Assert.AreEqual(1, f1.RefersToObjects.Length);
            Assert.AreEqual("FuncArgNTrack", f1.RefersToObjects[0].Name);
        }

        // TODO: NTrack(J2) is also an object definition, and the grammar can't
        // currently deal with that.
        [TestMethod]
        public void SingleLineFunctionRestrictionFSORef()
        {
            var text = "J2(atlas-jet); NTrack(J2, J2) = 0;";
            var dfs = text.Parse();

            Assert.AreEqual(1, dfs.FinalStateObjects.Count);
            Assert.AreEqual(1, dfs.Criteria.Arguments.Count);

            var s1 = dfs.Criteria.Arguments[0] as SelectionCriteria;
            Assert.IsInstanceOfType(s1.FirstArgument, typeof(FunctionPhysicalQuantity));
            Assert.IsInstanceOfType(s1.SecondArgument, typeof(PhysicalValue));
            Assert.AreEqual("=", s1.BinaryRelation);

            var f1 = s1.FirstArgument as FunctionPhysicalQuantity;
            Assert.AreEqual("NTrack", f1.Name);
            Assert.AreEqual("J2, J2", f1.ArgumentList);
            Assert.AreEqual(1, f1.RefersToObjects.Length);
            Assert.AreEqual("J2", f1.RefersToObjects[0].Name);
        }

        [TestMethod]
        public void SingleLineFunctionRestriction2FSORef()
        {
            var text = "J2(atlas-jet); J1(atlas-jet); NTrack(J2,J1) = 0;";
            var dfs = text.Parse();

            Assert.AreEqual(2, dfs.FinalStateObjects.Count);
            Assert.AreEqual(1, dfs.Criteria.Arguments.Count);

            var s1 = dfs.Criteria.Arguments[0] as SelectionCriteria;
            Assert.IsInstanceOfType(s1.FirstArgument, typeof(FunctionPhysicalQuantity));
            Assert.IsInstanceOfType(s1.SecondArgument, typeof(PhysicalValue));
            Assert.AreEqual("=", s1.BinaryRelation);

            var f1 = s1.FirstArgument as FunctionPhysicalQuantity;
            Assert.AreEqual("NTrack", f1.Name);
            Assert.AreEqual("J2, J1", f1.ArgumentList);
            Assert.AreEqual(2, f1.RefersToObjects.Length);
            Assert.AreEqual("J2", f1.RefersToObjects[0].Name);
            Assert.AreEqual("J1", f1.RefersToObjects[1].Name);
        }

        [TestMethod]
        public void SingleLineFunctionRestrictionFSOCutRef()
        {
            var text = "J2(atlas-jet); NTrack(J2.pT>10 GeV) = 0;";
            var dfs = text.Parse();

            Assert.AreEqual(1, dfs.FinalStateObjects.Count);
            Assert.AreEqual(1, dfs.Criteria.Arguments.Count);

            var s1 = dfs.Criteria.Arguments[0] as SelectionCriteria;
            Assert.IsInstanceOfType(s1.FirstArgument, typeof(FunctionPhysicalQuantity));
            Assert.IsInstanceOfType(s1.SecondArgument, typeof(PhysicalValue));
            Assert.AreEqual("=", s1.BinaryRelation);

            var f1 = s1.FirstArgument as FunctionPhysicalQuantity;
            Assert.AreEqual("NTrack", f1.Name);
            Assert.AreEqual("J2.pT>10GeV", f1.ArgumentList);
            Assert.AreEqual(1, f1.RefersToObjects.Length);
            Assert.AreEqual("J2", f1.RefersToObjects[0].Name);
        }

        [TestMethod]
        public void SingleLineFunctionRestrictionFSO2CutRef()
        {
            var text = "J1 (atlas-jet); J2(atlas-jet); NTrack(J2.pT>10 GeV, J1.ET>20 GeV) = 0;";
            var dfs = text.Parse();

            Assert.AreEqual(2, dfs.FinalStateObjects.Count);
            Assert.AreEqual(1, dfs.Criteria.Arguments.Count);

            var s1 = dfs.Criteria.Arguments[0] as SelectionCriteria;
            Assert.IsInstanceOfType(s1.FirstArgument, typeof(FunctionPhysicalQuantity));
            Assert.IsInstanceOfType(s1.SecondArgument, typeof(PhysicalValue));
            Assert.AreEqual("=", s1.BinaryRelation);

            var f1 = s1.FirstArgument as FunctionPhysicalQuantity;
            Assert.AreEqual("NTrack", f1.Name);
            Assert.AreEqual("J2.pT>10GeV, J1.ET>20GeV", f1.ArgumentList);
            Assert.AreEqual(2, f1.RefersToObjects.Length);
            Assert.AreEqual("J2", f1.RefersToObjects[0].Name);
            Assert.AreEqual("J1", f1.RefersToObjects[1].Name);
        }

        [TestMethod]
        public void FunctionWithCutArgument()
        {
            var text = "J2(atlas-jet); NTrack(DR=0.2) = 0;";
            var dfs = text.Parse();

            Assert.AreEqual(1, dfs.FinalStateObjects.Count);
            Assert.AreEqual(1, dfs.Criteria.Arguments.Count);

            var s1 = dfs.Criteria.Arguments[0] as SelectionCriteria;
            var f1 = s1.FirstArgument as FunctionPhysicalQuantity;

            Assert.AreEqual(1, f1.RefersToObjects.Length);
            Assert.AreEqual("FuncArgNTrack", f1.RefersToObjects[0].Name);
        }

        [TestMethod]
        public void FunctionWithMultipleArguments()
        {
            var text = "J2(atlas-jet); NTrack(J2, DR=0.2, pT>1) = 0;";
            var dfs = text.Parse();

            Assert.AreEqual(1, dfs.FinalStateObjects.Count);
            Assert.AreEqual(1, dfs.Criteria.Arguments.Count);

            var s1 = dfs.Criteria.Arguments[0] as SelectionCriteria;
            var f1 = s1.FirstArgument as FunctionPhysicalQuantity;

            Assert.AreEqual(2, f1.RefersToObjects.Length);
            Assert.AreEqual("J2", f1.RefersToObjects[0].Name);
            Assert.AreEqual("FuncArgNTrack", f1.RefersToObjects[1].Name);
            Assert.AreEqual("J2, DR=0.2, pT>1", f1.ArgumentList);
        }

        [TestMethod]
        public void SingleLineFunctionWithCutAndObjRef()
        {
            var text = "J2(atlas-jet); NTrack(J2.pT > 30 GeV, DR=0.2, pT>1) = 0;";
            var dfs = text.Parse();

            Assert.AreEqual(1, dfs.FinalStateObjects.Count);
            Assert.AreEqual(1, dfs.Criteria.Arguments.Count);

            var s1 = dfs.Criteria.Arguments[0] as SelectionCriteria;
            var f1 = s1.FirstArgument as FunctionPhysicalQuantity;

            Assert.AreEqual(2, f1.RefersToObjects.Length);
            Assert.AreEqual("J2", f1.RefersToObjects[0].Name);
            Assert.AreEqual("FuncArgNTrack", f1.RefersToObjects[1].Name);
        }

        /// <summary>
        /// Make sure we are not dealing with criteria within criteria and their definition. :-)
        /// </summary>
        [TestMethod]
        public void FunctionInObjSpecification()
        {
            // TODO: (WARNING) - this works, but DR might be a function argument, not an argument
            // of J2 - the parser needs to know more than semantics... it needs a model to help
            // understand what is going on here.

            var text = "J2(atlas-jet) : NTrack(J2.pT > 30 GeV, DR=0.2, pT>1) = 0;";
            var dfs = text.Parse();

            Assert.AreEqual(1, dfs.FinalStateObjects.Count);
            Assert.AreEqual(1, dfs.Criteria.Arguments.Count);

            var s1 = dfs.Criteria.Arguments[0] as SelectionCriteria;
            var f1 = s1.FirstArgument as FunctionPhysicalQuantity;

            Assert.AreEqual(1, f1.RefersToObjects.Length);
            Assert.AreEqual("J2", f1.RefersToObjects[0].Name);
        }

        [TestMethod]
        public void TwoFunctionsInSingleCut()
        {
            var text = "J2(atlas-jet); J1(atlas-jet); NTrack(J2.pT > 30 GeV, DR=0.2, pT>1) = NTK(J1.pT > 30 GeV);";
            var dfs = text.Parse();

            Assert.AreEqual(2, dfs.FinalStateObjects.Count);
            Assert.AreEqual(1, dfs.Criteria.Arguments.Count);

            var s1 = dfs.Criteria.Arguments[0] as SelectionCriteria;
            var f1 = s1.FirstArgument as FunctionPhysicalQuantity;
            var f2 = s1.SecondArgument as FunctionPhysicalQuantity;

            Assert.AreEqual(2, f1.RefersToObjects.Length);
            Assert.AreEqual("J2", f1.RefersToObjects[0].Name);
            Assert.AreEqual("FuncArgNTrack", f1.RefersToObjects[1].Name);
            Assert.AreEqual("J2.pT>30GeV, DR=0.2, pT>1", f1.ArgumentList);

            Assert.AreEqual(1, f2.RefersToObjects.Length);
            Assert.AreEqual("J1", f2.RefersToObjects[0].Name);
            Assert.AreEqual("J1.pT>30GeV", f2.ArgumentList);
        }
    }
}
