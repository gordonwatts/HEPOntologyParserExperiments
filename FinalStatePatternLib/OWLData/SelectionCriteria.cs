
namespace FinalStatePatternLib.OWLData
{
    /// <summary>
    /// Represents a standard comparions or selection criteria.
    /// </summary>
    public class SelectionCriteria : ISelectionCriteriaBase
    {
        public IValueBase FirstArgument;
        public string BinaryRelation;
        public IValueBase SecondArgument;

        /// <summary>
        /// Mostly b.c. it is useful in debugging!
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2}", FirstArgument, BinaryRelation, SecondArgument);
        }
    }
}
