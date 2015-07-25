using FinalStatePatternLib.OWLData;
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
            wr.WriteLine("<#{0}> rdf:type dfs:PhysicsObject {1}", fso.Name, fso.BaseDefinition == null ? "." : fso.BaseDefinition);
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
