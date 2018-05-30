using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UtilityManager.Decisions;

namespace UtilityManager
{
    /// <summary>
    /// Takes a list of considerations to tabulate for a score. Runs the score against the Response Curve.
    /// </summary>
    [Serializable]
    public class DSE : IDSE
    {
        [SerializeField]
        private string _name;
        public string Name { get { return _name; } }
        [SerializeField]
        private string _description;
        [SerializeField]
        private DecisionEnum _decisionEnum;
        [SerializeField]
        private float _weight = 1f;
        public float Weight { get { return _weight; } set { _weight = value; } }
        [SerializeField]
        private bool _runForAllTargets = false;
        public bool RunForAllTargets { get { return _runForAllTargets; } }
        [SerializeField]
        private bool _lockUntilComplete;        
        public bool LockUntilComplete { get { return _lockUntilComplete; } }
        [SerializeField]
        private List<Consideration> _considerations;
        //public IEnumerable<IConsideration> Considerations { get { return _considerations.Select(i => (IConsideration)i); } set { _considerations = value.Select(i => (Consideration)i).ToList(); } }
        public IEnumerable<IConsideration> Considerations { get; set; }
        [SerializeField]
        private FsmTemplate _fsmTemplate;
        public FsmTemplate FsmTemplate { get { return _fsmTemplate; } }
        [SerializeField]
        private BehaviorDesigner.Runtime.ExternalBehaviorTree _behaviorTree;
        private IDecision _decision;

        public void Init()
        {
            Considerations = _considerations.Select(i => (IConsideration)i);
            _decision = DecisionFactory.Create(_decisionEnum);
        }
        
        //public DecisionBase decision;

        /// <summary>
        /// Takes a list of considerations to tabulate for a score. Runs the score against the Response Curve.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="bonus"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public float Score(IDecisionContext context, float bonus, float min)
        {
            float finalScore = bonus;
            if (Considerations == null)
                Init();
            if (Considerations == null)
            {
                Debug.LogError("DSE " + this.Name + " has no decisions");
                return 0f;
            }
            foreach (var consideration in Considerations)
            {
                if (!consideration.IsActive)
                    continue;
                // Can't possibly get above min
                if ((0.0f > finalScore) || (finalScore < min))
                    return 0f;
                    //break;
                float score = consideration.Score(context, consideration.Parameters);
                float response = consideration.ComputeResponseCurve(score);
                finalScore *= Mathf.Clamp01(response);
            }
            finalScore = MakeUpFactor(Considerations.Count(), (float) finalScore / bonus) * bonus;
            return finalScore;
        }

        private float MakeUpFactor(int considerationCount, float score)
        {
            float modificationFactor = 1f - (1f / considerationCount);
            float makeUpValue = (1f - score) * modificationFactor;
            float finalConsiderationScore = score + (makeUpValue * score);
            return finalConsiderationScore;
        }

        public void RunDecision(IDecisionContext context)
        {
            _decision.Start(context, this);
        }

        public bool IsFinished()
        {
            return _decision.IsFinished();
        }

        public void Stop(IDecisionContext context)
        {
            _decision.End(context, this);
        }

        public bool IsDSELocked()
        {
            //var dse = _currentDSExTarget.GetDSE();
            if (LockUntilComplete && !_decision.IsFinished())
                return true;
            return false;
        }

        //public float GetWeight()
        //{
        //    return _weight;
        //}
    }
}
