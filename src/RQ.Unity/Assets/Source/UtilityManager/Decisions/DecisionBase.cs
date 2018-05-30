using RQ.Entity.Components;
using RQ.Physics.Components;

namespace UtilityManager.Decisions
{
    public abstract class DecisionBase : IDecision// : ScriptableObject
    {
        public abstract void Start(IDecisionContext context, IDSE dse);
        public abstract void End(IDecisionContext context, IDSE dse);
        private bool _isTemplate;
        private PlayMakerStateMachineComponent _pmComponent;
        //public bool IsFinished { get; set; }

        protected void RunTemplate(IComponentRepository entity, FsmTemplate template)
        {
            _isTemplate = true;
            _pmComponent = entity.Components.GetComponent<PlayMakerStateMachineComponent>();
            _pmComponent.SetTemplate(template);
            _pmComponent.StartFsm();
        }

        public bool IsFinished()
        {
            if (_isTemplate)
            {
                return _pmComponent.IsFinished();
            }
            return false;
        }

        protected void StopTemplate(IComponentRepository entity, FsmTemplate template)
        {
            //var pmComponent = entity.Components.GetComponent<PlayMakerStateMachineComponent>();
            _pmComponent.StopFsm();
        }
    }
}
