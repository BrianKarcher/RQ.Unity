using RQ.AI.Action;
using RQ.Entity.AtomAction.Action;
using RQ.Entity.Components;
using RQ.Physics.Components;
using UnityEngine;

namespace Assets.Source.AI.Unity_State_Machine
{
    public class DamageBounce : StateMachineBehaviour
    {
        [SerializeField]
        private string _successTrigger;
        [SerializeField]
        public DamageBounceAtom _damageBounceAtom;
        [SerializeField]
        public PlayDamageSoundAtom _playSoundOneShotAtom;
        private IComponentRepository _entity;
        private bool _isRunning;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            var usm = animator.GetComponent<UnityStateMachineComponent>();
            _entity = usm.GetComponentRepository();
            if (_damageBounceAtom == null)
                _damageBounceAtom = new DamageBounceAtom();
            _damageBounceAtom.Start(_entity);
            _playSoundOneShotAtom.Start(_entity);
            _isRunning = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            _damageBounceAtom.End();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_isRunning)
                return;
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            var updateStatus = _damageBounceAtom.OnUpdate();
            if (updateStatus == RQ.AI.AtomActionResults.Success)
            {
                animator.SetTrigger(_successTrigger);
                _isRunning = false;
            }
        }
    }
}
