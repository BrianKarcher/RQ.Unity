using RQ.AI.Action;
using RQ.Entity.Components;
using RQ.Physics.Components;
using UnityEngine;

namespace Assets.Source.AI.Unity_State_Machine
{
    public class EnableBehaviorTree : StateMachineBehaviour
    {
        EnableBehaviorTreeAtom _enableBTAtom;
        private IComponentRepository _entity;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            var usm = animator.GetComponent<UnityStateMachineComponent>();
            _entity = usm.GetComponentRepository();
            if (_enableBTAtom == null)
                _enableBTAtom = new EnableBehaviorTreeAtom();
            _enableBTAtom.Start(_entity);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            _enableBTAtom.End();
        }
    }
}
