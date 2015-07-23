
namespace FinalStatePatternLib.OWLData
{
    /// <summary>
    /// Represents a physical value with a unit attached to it.
    /// </summary>
    public class PhysicalValue : IValueBase
    {
        public double Number;
        public string Unit = null;
    }
}
