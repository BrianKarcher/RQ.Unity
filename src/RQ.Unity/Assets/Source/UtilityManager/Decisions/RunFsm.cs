using RQ.Entity.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilityManager.Decisions
{
    public class RunFsm : DecisionBase
    {
        public override void Start(IDecisionContext context, IDSE dse)
        {
            base.RunTemplate(context.Self.Repo, dse.FsmTemplate);
        }

        public override void End(IDecisionContext context, IDSE dse)
        {
            base.StopTemplate(context.Self.Repo, dse.FsmTemplate);
        }
    }
}
