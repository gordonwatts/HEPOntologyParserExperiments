
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
        public List<FinalStateObject> FSOs = new List<FinalStateObject>();

        /// <summary>
        /// A new object specification line has been built. Record it and add it to our overall list.
        /// </summary>
        /// <param name="context"></param>
        public override void ExitObjectSpecNameOnly(FinalStatePatternParser.ObjectSpecNameOnlyContext context)
        {
            var fso_name = context.object_name().NAME().GetText();

            string fso_base_definition = null;
            if (context.object_name().base_definition() != null)
            {
                fso_base_definition = context.object_name().base_definition().GetText();
            }

            var oldFSO = FSOs.Where(f => f.Name == fso_name).FirstOrDefault();
            if (oldFSO != null)
            {
                if (oldFSO.BaseDefinition != fso_base_definition)
                {
                    throw new ArgumentOutOfRangeException(string.Format("Object {0} was defined with two base definitions ({1} and {2})", fso_name, fso_base_definition, oldFSO.BaseDefinition));
                }
            }
            else
            {
                var new_fso = new FinalStateObject() { Name = fso_name, BaseDefinition = fso_base_definition };
                FSOs.Add(new_fso);
            }

            // Go on to what we were doing previously.
            base.ExitObjectSpecNameOnly(context);
        }
    }
}
