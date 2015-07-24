
namespace FinalStatePatternLib.OWLData
{
    class FunctionPhysicalQuantity : IValueBase
    {
        public string Name;
        public string ArgumentList;
        public string[] RefersToObjects;

        public override string ToString()
        {
            return string.Format("{0}({1})", Name, ArgumentList == null ? "" : ArgumentList);
        }
    }
}
