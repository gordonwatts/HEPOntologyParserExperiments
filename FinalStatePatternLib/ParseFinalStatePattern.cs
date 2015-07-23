
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FinalStatePatternLib.Listeners;
using FinalStatePatternLib.OWLData;
using System;
using System.Collections.Generic;
namespace FinalStatePatternLib
{
    public static class ParseFinalStatePattern
    {
        /// <summary>
        /// Return a DFS parsed from the text. Throw if error during parsing.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static DetectorFinalState Parse(this string text)
        {
            // Lex.
            var input = new AntlrInputStream(text);
            var lexer = new FinalStatePatternLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);

            // Parse, capture errors.
            var parser = new FinalStatePatternParser(tokens);

            var errors = new ErrorRecorder();
            parser.AddErrorListener(errors);

            var tree = parser.top_level();

            if (errors.Errors.Count > 0)
            {
                string msg = "";
                foreach (var e in errors.Errors)
                {
                    msg += e;
                }
                throw new ArgumentException(msg);
            }

            // Convert to DFS
            var traverser = new FinalStatePatternListener();
            var walker = new ParseTreeWalker();
            walker.Walk(traverser, tree);

            // And collect the DFS

            return traverser.BuildDFS();

        }

        /// <summary>
        /// Capture errors and record them.
        /// </summary>
        private class ErrorRecorder : IAntlrErrorListener<IToken>
        {
            /// <summary>
            /// List of error messages that have occured.
            /// </summary>
            public List<string> Errors = new List<string>();

            public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            {
                Errors.Add(string.Format("Line: {0}, Char: {1} ({3}): {2} ", line, charPositionInLine, msg, offendingSymbol.Text));
            }
        }

        /// <summary>
        /// Given a listener that has traversed the tree, extract all the data into a DFS.
        /// </summary>
        /// <param name="parsedState"></param>
        /// <returns></returns>
        private static DetectorFinalState BuildDFS(this FinalStatePatternListener parsedState)
        {
            var result = new DetectorFinalState();
            result.FinalStateObjects.AddRange(parsedState.FSOs);
            result.Criteria.Arguments.AddRange(parsedState.TopLevelCriteria);

            return result;
        }

    }
}
