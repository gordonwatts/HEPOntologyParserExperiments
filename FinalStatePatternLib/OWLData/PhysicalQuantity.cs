
namespace FinalStatePatternLib.OWLData
{
    class SinglePhysicalQuantity : IValueBase
    {
        /// <summary>
        /// Get/Set the name of the object that this guy refers to
        /// </summary>
        public string RefersToObject;

        /// <summary>
        /// What about the object (pT, eta, etc.)
        /// </summary>
        public string PhysicalQantity;
    }
}
