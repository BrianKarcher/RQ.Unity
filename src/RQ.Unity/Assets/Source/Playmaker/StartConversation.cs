using PM = HutongGames.PlayMaker;
using HutongGames.PlayMaker;
using RQ.Animation.BasicAction.Action;
using RQ.Entity.Components;
using RQ.Physics.Components;
using RQ.AI.Action;
using RQ.AI.Atom.UI;

namespace RQ.AI.PlayMaker.UI
{
    [ActionCategory("RQ.UI")]
    [PM.Tooltip("Begins a conversation via a ConversationTriggerComponent.")]
    public class StartConversation : FsmStateAction
    {
        public StartConversationAtom _atom;
        private IComponentRepository _entity;

        public override void OnEnter()
        {
            var rqSM = Owner.GetComponent<PlayMakerStateMachineComponent>();
            _entity = rqSM.GetComponentRepository();
            _atom.Start(_entity);
            Finish();
        }

        public override void OnExit()
        {
            base.OnExit();
            _atom.End();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            var result = _atom.OnUpdate();
        }
    }
}
