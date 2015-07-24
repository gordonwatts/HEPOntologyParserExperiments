
using FinalStatePatternLib.OWLData;
using System;
using System.Collections.Generic;
using System.Linq;


namespace FinalStatePatternLib.Listeners
{
    /// <summary>
    /// Main listener for the grammar parsing.
    /// </summary>
    /// <remarks>
    /// Since we want to accumulate everything, rather than calculate, the listener seems to be a better choice than a vistior.
    /// Especially considering the different types that are floating around.
    /// However, it does mean that there will be lots of global variables implemented as instance variables in here.
    /// </remarks>
    class FinalStatePatternListener : FinalStatePatternBaseListener
    {
        /// <summary>
        /// List of all FSO's identified in this grammar.
        /// </summary>
        public List<FinalStateObject> FSOs = new List<FinalStateObject>();

        /// <summary>
        /// The top level criteria to be applied in defining this DFS.
        /// </summary>
        public List<ISelectionCriteriaBase> TopLevelCriteria = new List<ISelectionCriteriaBase>();

        /// <summary>
        /// A new object specification line has been built. Record it and add it to our overall list.
        /// No cuts are applied at this time in the grammer parsing.
        /// </summary>
        /// <param name="context"></param>
        public override void ExitObjectSpecNameOnly(FinalStatePatternParser.ObjectSpecNameOnlyContext context)
        {
            Convert(context.object_name());

            // Go on to what we were doing previously.
            base.ExitObjectSpecNameOnly(context);
        }

        private FinalStateObject _current_fso = null;
        private Stack<List<ISelectionCriteriaBase>> _current_criteria = new Stack<List<ISelectionCriteriaBase>>();

        /// <summary>
        /// Get ready to run by all the cuts that are listed on the command line.
        /// </summary>
        /// <param name="context"></param>
        public override void EnterObjectSpecNameAndCutList(FinalStatePatternParser.ObjectSpecNameAndCutListContext context)
        {
            // Cache the FSO for processing
            _current_fso = Convert(context.object_name());
            _current_criteria.Push(new List<ISelectionCriteriaBase>());

            base.EnterObjectSpecNameAndCutList(context);
        }

        /// <summary>
        /// When an object is defined, and a bunch of cuts are further required, we show up here.
        /// </summary>
        /// <param name="context"></param>
        public override void ExitObjectSpecNameAndCutList(FinalStatePatternParser.ObjectSpecNameAndCutListContext context)
        {
            // Pull the selection criteria out

            TopLevelCriteria.AddRange(_current_criteria.Pop());

            // Any downlevel processing.
            base.ExitObjectSpecNameAndCutList(context);
        }

        /// <summary>
        /// We are doing a stand alone cut on a single line; get things setup to track what happens on exit.
        /// </summary>
        /// <param name="context"></param>
        public override void EnterStandalone_cut(FinalStatePatternParser.Standalone_cutContext context)
        {
            _current_fso = null; // Should already be the case!
            _current_criteria.Push(new List<ISelectionCriteriaBase>());

            // Do the rest.
            base.EnterStandalone_cut(context);
        }

        /// <summary>
        /// Done processing the cuts, put them into our criteria list.
        /// </summary>
        /// <param name="context"></param>
        public override void ExitStandalone_cut(FinalStatePatternParser.Standalone_cutContext context)
        {
            TopLevelCriteria.AddRange(_current_criteria.Pop());

            // Continue.
            base.ExitStandalone_cut(context);
        }

        /// <summary>
        /// We see a binary cut, so put it on the list.
        /// </summary>
        /// <param name="context"></param>
        public override void ExitCutBinary(FinalStatePatternParser.CutBinaryContext context)
        {
            // If we are processing an argument, it isn't a criteria
            var c = Convert(context);
            _current_criteria.Peek().Add(c);

            // And off we go
            base.ExitCutBinary(context);
        }

        private SelectionCriteria Convert(FinalStatePatternParser.CutBinaryContext context)
        {
            var c = new SelectionCriteria();
            c.BinaryRelation = context.BINARY_OP().GetText();
            c.FirstArgument = Convert(context.cut_arg()[0]);
            c.SecondArgument = Convert(context.cut_arg()[1]);
            return c;
        }

        /// <summary>
        /// The user has given us a range cut
        /// </summary>
        /// <param name="context"></param>
        public override void ExitCutRange(FinalStatePatternParser.CutRangeContext context)
        {
            var c1 = new SelectionCriteria();
            var c2 = new SelectionCriteria();

            c1.BinaryRelation = context.BINARY_OP(0).GetText();
            c2.BinaryRelation = context.BINARY_OP(1).GetText();

            c1.FirstArgument = Convert(context.cut_number(0));
            c1.SecondArgument = Convert(context.cut_name());

            c2.FirstArgument = Convert(context.cut_name());
            c2.SecondArgument = Convert(context.cut_number(1));

            _current_criteria.Peek().Add(c1);
            _current_criteria.Peek().Add(c2);

            base.ExitCutRange(context);
        }

        /// <summary>
        /// Convert a argument to a cut (a single "term") into a IValueBase
        /// </summary>
        /// <param name="cut_argContext"></param>
        /// <returns></returns>
        private IValueBase Convert(FinalStatePatternParser.Cut_argContext cut_argContext)
        {
            if (cut_argContext.cut_name() != null)
            {
                return Convert(cut_argContext.cut_name());
            }
            else if (cut_argContext.cut_number() != null)
            {
                return Convert(cut_argContext.cut_number());
            }
            else if (cut_argContext.function() != null)
            {
                // Should have already been parsed.
                return _parsed_functions.Dequeue();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Cache the cut when we are doing a function as some special rules can apply.
        /// </summary>
        private FinalStateObject _current_cut = null;

        private Queue<IValueBase> _parsed_functions = new Queue<IValueBase>();

        /// <summary>
        /// When we enter a function argument list processing, make sure to specify what is needed.
        /// </summary>
        /// <param name="context"></param>
        public override void EnterFunction(FinalStatePatternParser.FunctionContext context)
        {
            _current_cut = new FinalStateObject() { Name = string.Format("FuncArg{0}", context.NAME().GetText()) };
            _current_criteria.Push(new List<ISelectionCriteriaBase>());
            base.EnterFunction(context);
        }

        /// <summary>
        /// When we leave the function, clean up so we don't accidentally re-use the
        /// function context.
        /// </summary>
        /// <param name="context"></param>
        public override void ExitFunction(FinalStatePatternParser.FunctionContext context)
        {
            _current_cut = null;
            _parsed_functions.Enqueue(Convert(context));
            _current_criteria.Pop();
            base.ExitFunction(context);
        }

        /// <summary>
        /// Type type of FSO reference we are converting. In one case, if it is defined twice, it must be the same!
        /// </summary>
        enum AllowedFSODefinitionReference
        {
            kAsDefinitionOnly,
            kAsDefinitionOrReference
        }

        /// <summary>
        /// Given an object name context, extract a FSO.
        /// </summary>
        /// <param name="objNameContext"></param>
        private FinalStateObject Convert(FinalStatePatternParser.Object_nameContext objNameContext, AllowedFSODefinitionReference refType = AllowedFSODefinitionReference.kAsDefinitionOnly)
        {
            var fso_name = objNameContext.NAME().GetText();

            string fso_base_definition = null;
            if (objNameContext.base_definition() != null)
            {
                fso_base_definition = objNameContext.base_definition().GetText();
            }

            var oldFSO = FSOs.Where(f => f.Name == fso_name).FirstOrDefault();
            if (oldFSO != null)
            {
                if (refType == AllowedFSODefinitionReference.kAsDefinitionOnly && oldFSO.BaseDefinition != fso_base_definition)
                {
                    throw new ArgumentOutOfRangeException(string.Format("Object {0} was defined with two base definitions ({1} and {2})", fso_name, fso_base_definition, oldFSO.BaseDefinition));
                }
                return oldFSO;
            }
            else
            {
                var new_fso = new FinalStateObject() { Name = fso_name, BaseDefinition = fso_base_definition };
                FSOs.Add(new_fso);
                return new_fso;
            }
        }

        /// <summary>
        /// Convert a cut_name to a IValueBase
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private IValueBase Convert(FinalStatePatternParser.Cut_nameContext func)
        {
            // Defaults, depending on context
            // This is what happens when the same code is used for multiple rules. :(
            var fso = _current_fso;
            if (fso == null)
            {
                fso = _current_cut;
            }

            // See if the fso was explicitly defined.
            if (func.object_name() != null)
            {
                fso = Convert(func.object_name(), AllowedFSODefinitionReference.kAsDefinitionOrReference);
            }

            if (fso == null)
            {
                throw new ArgumentException(string.Format("Unable to figure out what object this value is refering to: {0}", func.NAME().GetText()));
            }

            // It is possible to have a name like ETMiss as a stand-alone name.
            // However, then the fso must be defined.
            // (see above)... And we really need to double check this.
            // TODO: Alias mechanism so ETMiss => ETMiss.ET or similar?
            // Or we can use MET or something like that - so most physicists will
            // end up doing the "right thing".
            var name = func.NAME() == null ? "" : func.NAME().GetText();

            return new SinglePhysicalQuantity()
            {
                PhysicalQantity = name,
                RefersToObject = fso
            };
        }

        /// <summary>
        /// Convert a cut number to a IValueBase
        /// </summary>
        /// <param name="cut_numberContext"></param>
        /// <returns></returns>
        private IValueBase Convert(FinalStatePatternParser.Cut_numberContext cut_numberContext)
        {
            return new PhysicalValue()
            {
                Number = double.Parse(cut_numberContext.NUMBER().GetText()),
                Unit = cut_numberContext.unit() == null ? null : cut_numberContext.unit().GetText()
            };
        }

        /// <summary>
        /// Convert a function
        /// </summary>
        /// <param name="functionContext"></param>
        /// <returns></returns>
        private IValueBase Convert(FinalStatePatternParser.FunctionContext functionContext)
        {
            // All the arguments are either on the stack as selection criteria or burried deep.
            var allObjectsInContext =
                functionContext.function_arg()
                .SelectMany(fa => ExtractFSOReferences(fa));
            var allObjectsOnStack =
                _current_criteria.Peek()
                .SelectMany(fa => ExtractFSOReferences(fa));
            var seenObjectNames = new HashSet<FinalStateObject>();
            foreach (var anObject in allObjectsInContext.Concat(allObjectsOnStack))
            {
                seenObjectNames.Add(anObject);
            }

            var textItems =
                functionContext
                .function_arg()
                .Select(a => a.GetText());
            string arglist = "";
            foreach (var at in textItems)
            {
                if (arglist.Length > 0)
                    arglist += ", ";
                arglist += at;
            }

            return new FunctionPhysicalQuantity()
            {
                Name = functionContext.NAME().GetText(),
                ArgumentList = arglist,
                RefersToObjects = seenObjectNames.ToArray()
            };
        }

        /// TODO: This extraction code is horrible, and is already discovered by parsing. We should be able to just
        /// AVOID IT. Make the parser better when we learn how to do it.

        /// <summary>
        /// Given an argument, see if there are any FSO's in there.
        /// </summary>
        /// <param name="fa"></param>
        /// <returns></returns>
        private IEnumerable<FinalStateObject> ExtractFSOReferences(FinalStatePatternParser.Function_argContext fa)
        {
            if (fa.cut() != null)
            {
                return ExtractFSOReferences(fa.cut());
            }
            else if (fa.cut_name() != null)
            {
                return ExtractFSOReferences(fa.cut_name());
            }
            else
            {
                throw new InvalidOperationException("Unable to extra information from a function argument.");
            }
        }

        /// <summary>
        /// Given a cut name, see if there are any references to various FSO's.
        /// </summary>
        /// <param name="cut_nameContext"></param>
        /// <returns></returns>
        private IEnumerable<FinalStateObject> ExtractFSOReferences(FinalStatePatternParser.Cut_nameContext cut_nameContext)
        {
            if (cut_nameContext.object_name() != null)
            {
                yield return Convert(cut_nameContext.object_name());
            }
            else if (cut_nameContext.NAME() != null)
            {
                var nm = cut_nameContext.NAME().GetText();
                var f = FSOs.Where(fs => fs.Name == nm).FirstOrDefault();
                if (f != null)
                {
                    yield return f;
                }
            }
        }

        /// <summary>
        /// Extract FSO references
        /// </summary>
        /// <param name="cutContext"></param>
        /// <returns></returns>
        private IEnumerable<FinalStateObject> ExtractFSOReferences(FinalStatePatternParser.CutContext cutContext)
        {
            return _current_criteria.Peek()
                .SelectMany(sc => ExtractFSOReferences(sc));
        }

        private IEnumerable<FinalStateObject> ExtractFSOReferences(ISelectionCriteriaBase sc)
        {
            if (sc is SelectionCriteria)
            {
                var fsc = sc as SelectionCriteria;
                return ExtractFSOReference(fsc.FirstArgument)
                    .Concat(ExtractFSOReference(fsc.SecondArgument));
            }
            else if (sc is ANDOR)
            {
                return (sc as ANDOR)
                    .Arguments
                    .SelectMany(a => ExtractFSOReferences(a));
            }
            else
            {
                throw new InvalidOperationException("Unknown selectrion criteria");
            }
        }

        /// <summary>
        /// Extract the FSO references.
        /// </summary>
        /// <param name="valueBase"></param>
        /// <returns></returns>
        private IEnumerable<FinalStateObject> ExtractFSOReference(IValueBase valueBase)
        {
            if (valueBase is PhysicalValue)
            {
                return ExtractFSOReferences(valueBase as PhysicalValue);
            }
            else if (valueBase is SinglePhysicalQuantity)
            {
                return ExtractFSOReferences(valueBase as SinglePhysicalQuantity);
            }
            else if (valueBase is FunctionPhysicalQuantity)
            {
                return ExtractFSOReferences(valueBase as FunctionPhysicalQuantity);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Return a function's referenced items.
        /// </summary>
        /// <param name="functionPhysicalQuantity"></param>
        /// <returns></returns>
        private IEnumerable<FinalStateObject> ExtractFSOReferences(FunctionPhysicalQuantity functionPhysicalQuantity)
        {
            return functionPhysicalQuantity.RefersToObjects;
        }

        /// <summary>
        /// List the various items
        /// </summary>
        /// <param name="singlePhysicalQuantity"></param>
        /// <returns></returns>
        private IEnumerable<FinalStateObject> ExtractFSOReferences(SinglePhysicalQuantity singlePhysicalQuantity)
        {
            if (singlePhysicalQuantity.RefersToObject != null)
                return new FinalStateObject[] { singlePhysicalQuantity.RefersToObject };
            return Enumerable.Empty<FinalStateObject>();
        }

        /// <summary>
        /// A physical value has nothing.
        /// </summary>
        /// <param name="physicalValue"></param>
        /// <returns></returns>
        private IEnumerable<FinalStateObject> ExtractFSOReferences(PhysicalValue physicalValue)
        {
            return Enumerable.Empty<FinalStateObject>();
        }


    }
}
