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
        public static void Emit(this FinalStateObject fso, TextWriter wr)
        {
            wr.WriteLine("<#{0}> rdf:type dfs:PhysicsObject {1}", fso.Name, fso.BaseDefinition == null ? "." : fso.BaseDefinition);
        }
    }
}
