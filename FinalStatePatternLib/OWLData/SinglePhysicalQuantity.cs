
namespace FinalStatePatternLib.OWLData
{
    public class SinglePhysicalQuantity : IValueBase
    {
        /// <summary>
        /// Get/Set the name of the object that this guy refers to
        /// </summary>
        public FinalStateObject RefersToObject;

        /// <summary>
        /// What about the object (pT, eta, etc.)
        /// </summary>
        public string PhysicalQantity;

        public override string ToString()
        {
            return string.Format("{0}{1}", RefersToObject, PhysicalQantity == null ? "" : "." + PhysicalQantity);
        }
    }
}
