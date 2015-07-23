
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
        private List<ISelectionCriteriaBase> _current_criteria = null;

        /// <summary>
        /// Get ready to run by all the cuts that are listed on the command line.
        /// </summary>
        /// <param name="context"></param>
        public override void EnterObjectSpecNameAndCutList(FinalStatePatternParser.ObjectSpecNameAndCutListContext context)
        {
            // Cache the FSO for processing
            _current_fso = Convert(context.object_name());
            _current_criteria = new List<ISelectionCriteriaBase>();

            base.EnterObjectSpecNameAndCutList(context);
        }

        /// <summary>
        /// When an object is defined, and a bunch of cuts are further required, we show up here.
        /// </summary>
        /// <param name="context"></param>
        public override void ExitObjectSpecNameAndCutList(FinalStatePatternParser.ObjectSpecNameAndCutListContext context)
        {
            // Pull the selection criteria out

            TopLevelCriteria.AddRange(_current_criteria);
            _current_criteria = null;

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
            _current_criteria = new List<ISelectionCriteriaBase>();

            // Do the rest.
            base.EnterStandalone_cut(context);
        }

        /// <summary>
        /// Done processing the cuts, put them into our criteria list.
        /// </summary>
        /// <param name="context"></param>
        public override void ExitStandalone_cut(FinalStatePatternParser.Standalone_cutContext context)
        {
            TopLevelCriteria.AddRange(_current_criteria);
            _current_criteria = null;

            // Continue.
            base.ExitStandalone_cut(context);
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
        /// We see a binary cut, so put it on the list.
        /// </summary>
        /// <param name="context"></param>
        public override void ExitCutBinary(FinalStatePatternParser.CutBinaryContext context)
        {
            var c = new SelectionCriteria();
            c.BinaryRelation = context.BINARY_OP().GetText();
            c.FirstArgument = Convert(context.self_ref_cut_arg()[0]);
            c.SecondArgument = Convert(context.self_ref_cut_arg()[1]);
            _current_criteria.Add(c);

            // And off we go
            base.ExitCutBinary(context);
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
            c1.SecondArgument = Convert(context.self_ref_cut_name());

            c2.FirstArgument = Convert(context.self_ref_cut_name());
            c2.SecondArgument = Convert(context.cut_number(1));

            _current_criteria.Add(c1);
            _current_criteria.Add(c2);

            base.ExitCutRange(context);
        }

        /// <summary>
        /// Convert a argument to a cut (a single "term") into a IValueBase
        /// </summary>
        /// <param name="cut_argContext"></param>
        /// <returns></returns>
        private IValueBase Convert(FinalStatePatternParser.Self_ref_cut_argContext cut_argContext)
        {
            if (cut_argContext.self_ref_cut_name() != null)
            {
                return Convert(cut_argContext.self_ref_cut_name());
            }
            else if (cut_argContext.cut_number() != null)
            {
                return Convert(cut_argContext.cut_number());
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Convert a cut_name to a IValueBase
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private IValueBase Convert(FinalStatePatternParser.Self_ref_cut_nameContext func)
        {
            var fso = _current_fso;
            if (func.object_name() != null)
            {
                fso = Convert(func.object_name(), AllowedFSODefinitionReference.kAsDefinitionOrReference);
            }

            if (fso == null)
            {
                throw new ArgumentException(string.Format("Unable to figure out what object this value is refering to: {0}", func.NAME().GetText()));
            }

            return new SinglePhysicalQuantity()
            {
                PhysicalQantity = func.NAME().GetText(),
                RefersToObject = fso.Name
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

    }
}
