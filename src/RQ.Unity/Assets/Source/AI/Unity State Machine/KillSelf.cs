using RQ.Animation.BasicAction.Action;
using RQ.Entity.AtomAction.Action;
using UnityEngine;

namespace Assets.Source.AI.Unity_State_Machine
{
    public class KillSelf : UnityStateMachineBase
    {
        [SerializeField]
        public KillSelfAtom _killSelfAtom;
        public PlayKillSoundAtom _playSoundOneShotAtom;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            _playSoundOneShotAtom.Start(GetEntity());
            _killSelfAtom.Start(GetEntity());            
        }
    }
}
