using RQ.Animation.BasicAction.Action;
using RQ.Entity.Components;
using RQ.Physics.Components;
using UnityEngine;

namespace Assets.Source.AI.Unity_State_Machine
{
    public class WaitUntilInViewport : StateMachineBehaviour
    {
        [SerializeField]
        private string _successTrigger;

        WaitUntilInViewportAtom _waitUntilInViewportAtom;
        private IComponentRepository _entity;
        private bool _isRunning;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _waitUntilInViewportAtom = new WaitUntilInViewportAtom();
            base.OnStateEnter(animator, stateInfo, layerIndex);
            var usm = animator.GetComponent<UnityStateMachineComponent>();
            _entity = usm.GetComponentRepository();
            _waitUntilInViewportAtom.Start(_entity);
            _isRunning = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            _waitUntilInViewportAtom.End();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_isRunning)
                return;
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            if (_waitUntilInViewportAtom.OnUpdate() == RQ.AI.AtomActionResults.Success)
            {
                animator.SetTrigger(_successTrigger);
                _isRunning = false;
            }
        }
    }
}
