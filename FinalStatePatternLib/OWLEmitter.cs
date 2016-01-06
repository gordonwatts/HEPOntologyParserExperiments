using FinalStatePatternLib.OWLData;
using System;
using System.IO;
using System.Linq;

namespace FinalStatePatternLib
{
    public static class OWLEmitter
    {
        /// <summary>
        /// Namespace we should be using for this emitter
        /// </summary>
        const string OWLNamespace = "atlas";

        /// <summary>
        /// Emit a FSO
        /// </summary>
        /// <param name="fso"></param>
        /// <param name="wr"></param>
        public static string Emit(this FinalStateObject fso, TextWriter wr)
        {
            var lineEnding = fso.BaseDefinition == null ? "." : ";";
            wr.WriteLine($"{OWLNamespace}:{fso.Name} rdf:type dfs:PhysicsObject {lineEnding}");
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
            wr.WriteLine($"{OWLNamespace}:{name} rdf:type dfs:NumericalValue ;");
            wr.Write($"  dfs:hasNumber \"{pv.Number}\"^^xsd:decimal ");
            if (pv.Unit == null)
            {
                wr.WriteLine(".");
            }
            else
            {
                wr.WriteLine(";");
                wr.WriteLine($"  dfs:hasUnit dfs:{pv.Unit} .");
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
            wr.WriteLine($"{OWLNamespace}:{name} rdf:type dfs:PhysicalQuantity ;");
            wr.WriteLine($"  dfs:refersToObject {OWLNamespace}:{pv.RefersToObject.Name} ;");
            wr.WriteLine($"  dfs:refersToFinalStateObjectProperty dfs:{pv.PhysicalQantity} .");
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
            wr.WriteLine($"{OWLNamespace}:{name} rdf:type dfs:PhysicalQuantity ;");
            wr.Write("  dfs:refersToObject ");
            bool first = true;
            foreach (var q in fv.RefersToObjects)
            {
                if (!first)
                {
                    wr.Write(" , ");
                }
                first = false;
                wr.Write($"{OWLNamespace}:{q.Name}");
            }
            wr.WriteLine(" ;");
            wr.WriteLine($"  dfs:hasQuantity \"{fv.Name}({fv.ArgumentList})\" .");
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
                throw new ArgumentException($"Do not know how to emit type '{v.GetType().Name}'");
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
            wr.WriteLine($"{OWLNamespace}:{name} rdf:type dfs:SelectionCriteria ;", name);
            wr.WriteLine($"  dfs:usesBinaryRelation dfs:{AsDFSBinaryRelation(sc.BinaryRelation)} ;");
            wr.WriteLine($"  dfs:hasFirstArgument {OWLNamespace}:{n1} ;");
            wr.WriteLine($"  dfs:hasSecondArgument {OWLNamespace}:{n2} .");
            return name;
        }

        /// <summary>
        /// Emit an AND/OR guy
        /// </summary>
        /// <param name="andor"></param>
        /// <param name="wr"></param>
        /// <returns></returns>
        public static string Emit(this ANDOR andor, TextWriter wr)
        {
            // Get the arguments out
            var criteria = andor.Arguments
                .Select(a =>
                {
                    var n = a.Emit(wr);
                    wr.WriteLine();
                    return n;
                })
                .ToArray();

            // Next our body.
            var name = MakeName("andor");
            var andorname = andor.AOType == ANDORType.kAnd ? "And" : "Or";
            wr.WriteLine($"{OWLNamespace}:{name} rdf:type dfs:{andorname} ;");
            wr.Write("  dfs:hasOperand ");
            bool first = true;
            foreach (var a in criteria)
            {
                if (!first)
                    wr.Write(" , ");
                first = false;
                wr.Write($"{OWLNamespace}:{a}");
            }
            wr.WriteLine(" .");

            return name;
        }

        /// <summary>
        /// Emit the top dog
        /// </summary>
        /// <param name="state"></param>
        /// <param name="wr"></param>
        /// <returns></returns>
        public static string Emit(this DetectorFinalState state, TextWriter wr)
        {
            foreach (var fso in state.FinalStateObjects)
            {
                fso.Emit(wr);
                wr.WriteLine();
            }
            var sname = state.Criteria.Emit(wr);
            wr.WriteLine();

            var name = MakeName("detectorFinalState");
            wr.WriteLine($"{OWLNamespace}:{name} rdf:type dfs:DetectorFinalState ;");
            wr.WriteLine($"  dfs:hasSelectionCriteria {OWLNamespace}:{sname} .");

            return name;
        }

        /// <summary>
        /// This will emit the headers required for OWL data.
        /// </summary>
        /// <param name="wr"></param>
        public static void EmitHeaders(TextWriter wr)
        {
            wr.WriteLine("@prefix owl: <http://www.w3.org/2002/07/owl#> .");
            wr.WriteLine("@prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> .");
            wr.WriteLine("@prefix xml: <http://www.w3.org/XML/1998/namespace> .");
            wr.WriteLine("@prefix xsd: <http://www.w3.org/2001/XMLSchema#> .");
            wr.WriteLine("@prefix rdfs: <http://www.w3.org/2000/01/rdf-schema#> .");
            wr.WriteLine("@prefix dfs: <https://w3id.org/daspos/detectorfinalstate#> .");
        }

        /// <summary>
        /// Emit all kinds of selection
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="wr"></param>
        /// <returns></returns>
        private static string Emit(this ISelectionCriteriaBase sb, TextWriter wr)
        {
            if (sb is SelectionCriteria)
            {
                return (sb as SelectionCriteria).Emit(wr);
            }
            else if (sb is ANDOR)
            {
                return (sb as ANDOR).Emit(wr);
            }
            else
            {
                throw new ArgumentException(string.Format("Don't know how to emit type '{0}'", sb.GetType()));
            }
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
