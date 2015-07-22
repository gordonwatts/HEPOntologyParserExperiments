﻿using FinalStatePatternLib;
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
    }
}
