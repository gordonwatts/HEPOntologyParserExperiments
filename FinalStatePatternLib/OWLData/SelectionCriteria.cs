
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
    }
}
