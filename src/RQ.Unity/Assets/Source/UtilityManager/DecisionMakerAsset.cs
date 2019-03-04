using RQ.Common.Config;
using System;
using UnityEngine;

namespace UtilityManager
{
    [Serializable]
    public class DecisionMakerAsset : RQBaseConfig
    {
        [SerializeField]
        private DecisionMaker _dm;

        /// <summary>
        /// Takes a list of considerations to tabulate for a score. Runs the score against the Response Curve.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="bonus"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        //public float Score(DecisionContext context, float bonus, float min)
        //{
        //    return dse.Score(context, bonus, min);
        //}
        public DecisionMaker GetDecisionMaker()
        {
            return _dm;
        }
    }
}
