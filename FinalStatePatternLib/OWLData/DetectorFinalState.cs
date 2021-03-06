﻿using System.Collections.Generic;

namespace FinalStatePatternLib.OWLData
{
    /// <summary>
    /// The top level Detector Final State
    /// </summary>
    public class DetectorFinalState
    {
        /// <summary>
        /// List of the final state objects used by this DFS
        /// </summary>
        public List<FinalStateObject> FinalStateObjects { get; private set; }

        /// <summary>
        /// All the criteria that need to be satisfied for this DFS.
        /// </summary>
        public ANDOR Criteria { get; private set; }

        /// <summary>
        /// Basic constructor
        /// </summary>
        public DetectorFinalState()
        {
            FinalStateObjects = new List<FinalStateObject>();
            Criteria = new ANDOR() { AOType = ANDORType.kAnd };
        }
    }
}
