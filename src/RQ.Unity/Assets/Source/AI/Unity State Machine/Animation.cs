using RQ.AI.Action;
using RQ.Animation.BasicAction.Action;
using RQ.Entity.Components;
using RQ.Physics.Components;
using UnityEngine;

namespace Assets.Source.AI.Unity_State_Machine
{
    public class Animation : StateMachineBehaviour
    {
        AnimationAtom _animationAtom;
        private IComponentRepository _entity;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            var usm = animator.GetComponent<UnityStateMachineComponent>();
            _entity = usm.GetComponentRepository();
            if (_animationAtom == null)
                _animationAtom = new AnimationAtom();
            _animationAtom.Start(_entity);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            _animationAtom.End();
        }
    }
}
