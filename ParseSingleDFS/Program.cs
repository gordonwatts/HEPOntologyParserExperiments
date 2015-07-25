using FinalStatePatternLib;
using System;
using System.IO;

namespace ParseSingleDFS
{
    class Program
    {
        /// <summary>
        /// Read from a given file, and output to std out.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: ParseSingleDFS <input-filename>");
                return;
            }
            var f = new FileInfo(args[0]);
            if (!f.Exists)
            {
                Console.WriteLine("File {0} does not exist!", f.FullName);
            }

            var text = "";
            using (var reader = f.OpenText())
            {
                text = reader.ReadToEnd();
            }

            try
            {
                var dfs = text.Parse();

                dfs.Emit(Console.Out);
            } catch (Exception e)
            {
                Console.WriteLine("Parse failed...");
            }
        }
    }
}
