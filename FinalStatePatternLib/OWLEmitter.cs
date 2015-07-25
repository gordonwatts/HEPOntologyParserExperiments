using FinalStatePatternLib.OWLData;
using System;
using System.IO;

namespace FinalStatePatternLib
{
    public static class OWLEmitter
    {
        /// <summary>
        /// Emit a detector final state
        /// </summary>
        /// <param name="dfs"></param>
        /// <param name="wr"></param>
        public static void Emit(this DetectorFinalState dfs, TextWriter wr)
        {

        }

        /// <summary>
        /// Emit a fso
        /// </summary>
        /// <param name="fso"></param>
        /// <param name="wr"></param>
        public static string Emit(this FinalStateObject fso, TextWriter wr)
        {
            wr.WriteLine("<#{0}> rdf:type dfs:PhysicsObject {1}", fso.Name, fso.BaseDefinition == null ? "." : ";");
            if (fso.BaseDefinition != null)
                wr.WriteLine("  hasBaseDefinition: \"{0}\" .", fso.BaseDefinition);
            return fso.Name;
        }

        /// <summary>
        /// Dump out a name
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="wr"></param>
        /// <returns></returns>
        public static string Emit(this PhysicalValue pv, TextWriter wr)
        {
            var name = MakeName("number");
            wr.WriteLine("<#{0}> rdf:type dfs:NumericalValue ;", name);
            wr.Write("  dfs:hasNumber \"{0}\"^^xsd:decimal ", pv.Number);
            if (pv.Unit == null)
            {
                wr.WriteLine(".");
            }
            else
            {
                wr.WriteLine(";");
                wr.WriteLine("  dfs:hasUnit dfs:{0} .", pv.Unit);
            }
            return name;
        }

        /// <summary>
        /// Emit a single physical quantity.
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="wr"></param>
        /// <returns></returns>
        public static string Emit(this SinglePhysicalQuantity pv, TextWriter wr)
        {
            var name = MakeName("physicalQuantity");
            wr.WriteLine("<#{0}> rdf:type dfs:PhysicalQuantity ;", name);
            wr.WriteLine("  dfs:refersToObject <#{0}> ;", pv.RefersToObject.Name);
            wr.WriteLine("  dfs:hasQuantity dfs:{0} .", pv.PhysicalQantity);
            return name;
        }

        /// <summary>
        /// Dump out a function reference
        /// </summary>
        /// <param name="fv"></param>
        /// <param name="wr"></param>
        /// <returns></returns>
        public static string Emit(this FunctionPhysicalQuantity fv, TextWriter wr)
        {
            var name = MakeName("functionQuantity");
            wr.WriteLine("<#{0}> rdf:type dfs:PhysicalQuantity ;", name);
            foreach (var q in fv.RefersToObjects)
            {
                wr.WriteLine("  dfs:refersToObject <#{0}> ;", q.Name);
            }
            wr.WriteLine("  dfs:hasQuantity \"{0}({1})\" .", fv.Name, fv.ArgumentList);
            return name;
        }

        /// <summary>
        /// General purpose IValueBase emitter.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="wr"></param>
        /// <returns></returns>
        public static string Emit(this IValueBase v, TextWriter wr)
        {
            if (v is PhysicalValue)
            {
                return (v as PhysicalValue).Emit(wr);
            }
            else if (v is SinglePhysicalQuantity)
            {
                return (v as SinglePhysicalQuantity).Emit(wr);
            }
            else if (v is FunctionPhysicalQuantity)
            {
                return (v as FunctionPhysicalQuantity).Emit(wr);
            }
            else
            {
                throw new ArgumentException(string.Format("Do not know how to emit type '{0}'", v.GetType().Name));
            }
        }

        /// <summary>
        /// Dump out a selection criteria.
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="wr"></param>
        /// <returns></returns>
        public static string Emit(this SelectionCriteria sc, TextWriter wr)
        {
            var n1 = sc.FirstArgument.Emit(wr);
            wr.WriteLine();
            var n2 = sc.SecondArgument.Emit(wr);
            wr.WriteLine();

            var name = MakeName("selectionCriteria");
            wr.WriteLine("<#{0}> rdf:type dfs:SelectionCriteria ;", name);
            wr.WriteLine("  dfs:usesBinaryRelation dfs:{0} ;", AsDFSBinaryRelation(sc.BinaryRelation));
            wr.WriteLine("  dfs:hasFirstArgument <#{0}> ;", n1);
            wr.WriteLine("  dfs:hasSecondArgument <#{0}> .", n2);
            return name;
        }

        private static string AsDFSBinaryRelation(string p)
        {
            if (p == ">")
            {
                return "greaterThan";
            }
            else if (p == "<")
            {
                return "lessThan";
            }
            else if (p == "=")
            {
                return "equal";
            }
            else if (p == ">=")
            {
                return "greaterEqual";
            }
            else if (p == "<=")
            {
                return "lessEqual";
            }
            else
            {
                throw new ArgumentException("Unknown operator: " + p);
            }
        }

        /// <summary>
        /// Track counter
        /// </summary>
        private static int g_counter = -1;

        /// <summary>
        /// Reset global variables. Useful for testing.
        /// </summary>
        public static void Reset()
        {
            g_counter = -1;
        }

        public static string MakeName(string baseName)
        {
            g_counter++;
            return string.Format("{0}{1}", baseName, g_counter);
        }
    }
}
