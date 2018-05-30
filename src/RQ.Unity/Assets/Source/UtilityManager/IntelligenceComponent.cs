using RQ.Common.Components;
using UnityEngine;

namespace UtilityManager
{
    [AddComponentMenu("RQ/Components/AI/Intelligence Component")]
    public class IntelligenceComponent : ComponentPersistence<IntelligenceComponent>
    {
        private IntelligenceBehavior _intelligenceBehavior;

        /// <summary>
        /// Doing this in Start to ensure that all entities are registered
        /// </summary>
        public override void Start()
        {
            base.Start();
            _intelligenceBehavior = GetComponent<IntelligenceBehavior>();
            _intelligenceBehavior.Init(GetComponentRepository());
        }
    }
}
