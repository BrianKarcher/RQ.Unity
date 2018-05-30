using RQ.Entity.Components;

namespace UtilityManager.Decisions
{
    public interface IDecision
    {
        void Start(IDecisionContext context, IDSE dse);
        void End(IDecisionContext context, IDSE dse);
        bool IsFinished();
    }
}