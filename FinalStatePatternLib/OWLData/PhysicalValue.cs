
namespace FinalStatePatternLib.OWLData
{
    /// <summary>
    /// Represents a physical value with a unit attached to it.
    /// </summary>
    public class PhysicalValue : IValueBase
    {
        public double Number;
        public string Unit = null;

        public override string ToString()
        {
            return string.Format("{0}{1}", Number, Unit == null ? "" : " " + Unit);
        }
    }
}
