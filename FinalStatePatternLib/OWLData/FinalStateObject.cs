
namespace FinalStatePatternLib.OWLData
{
    enum FinalStateObjectTypes
    {
        kPhysicsObject,
        kEventLevelQuantity,
        kUnknown
    }
    /// <summary>
    /// Contains the info for a final state object
    /// </summary>
    public class FinalStateObject
    {
        /// <summary>
        /// The name of the object (J1 or E1 or MET or similar).
        /// </summary>
        public string Name;

        /// <summary>
        /// The base definition
        /// </summary>
        /// <remarks>Can be null
        /// Something like "atlas-kt4-jet" or similar</remarks>
        public string BaseDefinition;

        /// <summary>
        /// What sort of type is this object?
        /// </summary>
        FinalStateObjectTypes ObjectType;

        /// <summary>
        /// Create an empty FSO.
        /// </summary>
        public FinalStateObject()
        {
            BaseDefinition = null;
            ObjectType = FinalStateObjectTypes.kUnknown;
            Name = null;
        }
    }
}
