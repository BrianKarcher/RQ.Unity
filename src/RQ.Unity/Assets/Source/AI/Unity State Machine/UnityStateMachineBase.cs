using RQ.Entity.Components;
using RQ.Physics.Components;
using System;
using UnityEngine;

namespace Assets.Source.AI.Unity_State_Machine
{
    [Serializable]
    public class UnityStateMachineBase : StateMachineBehaviour
    {
        private IComponentRepository _entity;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            var usm = animator.GetComponent<UnityStateMachineComponent>();
            _entity = usm.GetComponentRepository();
        }

        public IComponentRepository GetEntity()
        {
            return _entity;
        }
    }
}
