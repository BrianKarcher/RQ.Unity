using System.Collections.Generic;

namespace UtilityManager
{
    public interface IDecisionMaker
    {
        IEnumerable<IDSE> DSEList { get; }

        IEnumerable<IDSE> GetDSEList();
        void RunDecision(IDSE dse, IDecisionContext decisionContext);
        DSExTarget ScoreAllDecisions(IEnumerable<DSExTarget> decisions, IDecisionContext last);
    }
}