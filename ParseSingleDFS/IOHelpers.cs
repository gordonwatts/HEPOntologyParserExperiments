using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseSingleDFS
{
    /// <summary>
    /// Helper functions to make actually doing the work concise enough to understand in the main code.
    /// </summary>
    static class IOHelpers
    {
        /// <summary>
        /// Return a file lazy line reader that can be iterated over.
        /// </summary>
        /// <param name="rdr"></param>
        /// <returns></returns>
        public static IEnumerable<string> AsEnumerable (this TextReader rdr)
        {
            string line = null;
            while ((line = rdr.ReadLine()) != null)
            {
                yield return line;
            }
        }

        private static string LINEENDING = "\r\n";

        /// <summary>
        /// Take a list of lines and turn them into a giant string with proper line feed sequence between them.
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static string ReadAllLines (this IEnumerable<string> lines)
        {
            var s = new StringBuilder();

            bool first = true;
            foreach (var l in lines)
            {
                if (!first)
                {
                    s.Append(LINEENDING);
                }
                first = false;
                s.Append(l);
            }

            return s.ToString();
        }
    }
}
