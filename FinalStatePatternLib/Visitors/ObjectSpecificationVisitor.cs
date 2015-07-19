using FinalStatePatternLib.SemanticData;

namespace FinalStatePatternLib.Visitors
{
    /// <summary>
    /// Traverse the object specification rule (and only that rule! :-)).
    /// </summary>
    class ObjectSpecificationVisitor : FinalStatePatternBaseVisitor<ObjectSpecification>
    {
        /// <summary>
        /// Return an object specification that contains the info from our down-level parsing.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override ObjectSpecification VisitObject_specification(FinalStatePatternParser.Object_specificationContext context)
        {
            var result = new ObjectSpecification();
            return result;
            //return base.VisitObject_specification(context);
        }
    }
}
