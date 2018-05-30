using RQ.FSM.V2;

namespace UtilityManager.Decisions
{
    public class SetTargetRunFsm : RunFsm
    {
        public override void Start(IDecisionContext context, IDSE dse)
        {
            var aiComponent = context.Self.Repo.Components.GetComponent<AIComponent>();
            aiComponent.Target = context.EnemyEntity.Repo.transform;

            base.Start(context, dse);
        }
    }
}
