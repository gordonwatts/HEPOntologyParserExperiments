using Antlr4.Runtime;
using FinalStatePatternLib;
using FinalStatePatternLib.Visitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace t_FinalStatePatternLib
{
    [TestClass]
    public class ObjectSpecificationVisitorTest
    {
        /// <summary>
        /// Test a name object with no restrictions (e.g. just a definition).
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            var text = "J1";
        }

        [TestMethod]
        public void SimpleCut()
        {
            var text = "J1 : cut;";

            var input = new AntlrInputStream(text);
            var lexer = new FinalStatePatternLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            var parser = new FinalStatePatternParser(tokens);
            var listener = new errorListener();
            parser.AddErrorListener(listener);

            var tree = parser.top_level();
            listener.Check();

            var result = new ObjectSpecificationVisitor().Visit(tree);
            Assert.IsNotNull(result);
            Console.WriteLine("  -> Result: {0}", result);
        }

        class errorListener : IAntlrErrorListener<IToken>
        {
            public bool SeenError = false;
            string Message = "";

            public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
            {
                Console.WriteLine("Error: {0}", msg);
                SeenError = true;
                Message += string.Format("--> {0}\n", msg);
            }

            public void Check()
            {
                if (SeenError)
                {
                    Assert.Fail(Message);
                }
            }
        }

    }
}
