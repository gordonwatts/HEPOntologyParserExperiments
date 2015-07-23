
using System.Collections.Generic;
namespace FinalStatePatternLib.OWLData
{
    public enum ANDORType
    {
        kAnd, kOr
    };

    /// <summary>
    /// A list of criteria that should be and/or'd together.
    /// </summary>
    public class ANDOR : ISelectionCriteriaBase
    {
        /// <summary>
        /// Get/Set if each element in this list should be and/or or otherwise.
        /// </summary>
        public ANDORType AOType;

        /// <summary>
        /// All the criteria, etc., that need to be and/or'd together.
        /// </summary>
        public List<ISelectionCriteriaBase> Arguments { get; private set; }

        /// <summary>
        /// Simple setup
        /// </summary>
        public ANDOR()
        {
            Arguments = new List<ISelectionCriteriaBase>();
        }
    }
}
