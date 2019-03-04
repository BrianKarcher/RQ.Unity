using System;
using System.Collections.Generic;
using UnityEngine;

namespace UtilityManager
{
    /// <summary>
    /// Desicion Maker contains a list of DSE's and runs the score of all of them to arrive at a decision
    /// </summary>
    [Serializable]
    public class DecisionMaker : IDecisionMaker
    {
        [SerializeField]
        private string _name;
        [SerializeField]
        private string _description;
        [SerializeField]
        private List<DSEAsset> _dse;
        //public IEnumerable<IDSE> DSEList { get { return _dse.Select(i => (IDSE)i.GetDSE()); } }

        public IDSE[] GetDSEList()
        {
            var dseList = new IDSE[_dse.Count];
            for (int i = 0; i < _dse.Count; i++)
            {
                dseList[i] = (IDSE)_dse[i].GetDSE();
            }
            return dseList;
        }

        // Todo Make it a list of decisions instead of DSE's
        // Decisions are a DSE + target pair
        // Todo Have this simply tabulate a score, do not 
        public DSExTarget ScoreAllDecisions(IEnumerable<DSExTarget> decisions, IDecisionContext last)
        {
            DSExTarget chosenDSE = null;
            float cutoff = 0.0f;
            foreach (var decision in decisions)
            {
                float bonus = decision.GetDSE().Weight;
                if (bonus < cutoff)
                    continue;
                var score = decision.GetDSE().Score(decision.GetContext(), bonus, cutoff);
                if (score < 0.1f)
                    continue;
                if (score > cutoff)
                {
                    cutoff = score;
                    chosenDSE = decision;
                }
            }
            // Anything under 0.1f shouldn't be considered
            if (cutoff < 0.1f)
                return null;
            return chosenDSE;
        }

        public void RunDecision(IDSE dse, IDecisionContext decisionContext)
        {
            dse.RunDecision(decisionContext);
        }

        //public IEnumerable<IDSE> GetDSEList()
        //{
        //    return _dse.Select(i => (IDSE)i.GetDSE());
        //}
    }
}
