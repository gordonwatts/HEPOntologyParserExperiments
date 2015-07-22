using System.Collections.Generic;

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
        /// Basic ctor
        /// </summary>
        public DetectorFinalState()
        {
            FinalStateObjects = new List<FinalStateObject>();
        }
    }
}
