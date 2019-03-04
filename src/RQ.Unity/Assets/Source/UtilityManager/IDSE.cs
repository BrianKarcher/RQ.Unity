using System.Collections.Generic;

namespace UtilityManager
{
    public interface IDSE
    {
        string Name { get; }
        //IEnumerable<IConsideration> Considerations { get; set; }
        bool RunForAllTargets { get; }
        FsmTemplate FsmTemplate { get; }
        float Weight { get; set; }
        bool LockUntilComplete { get; }

        void Init();
        void RunDecision(IDecisionContext context);
        bool IsFinished();
        float Score(IDecisionContext context, float bonus, float min);
        void Stop(IDecisionContext context);
        bool IsDSELocked();
    }
}