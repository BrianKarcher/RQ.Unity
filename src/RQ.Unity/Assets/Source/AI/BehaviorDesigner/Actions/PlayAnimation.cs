using BehaviorDesigner.Runtime.Tasks;
using RQ.Animation.BasicAction.Action;
using RQ.Physics.Components;

namespace Assets.Source.AI.Actions
{
    [TaskDescription("Entity wanders aimlessly. The task will return running until the task is done waiting. It will return success after the wait time has elapsed.")]
    [TaskCategory("RQ")]
    public class PlayAnimation : Action
    {
        public AnimationAtom animationAtom;

        public override void OnStart()
        {
            base.OnStart();
            var entity = GetComponent<BehaviorTreeComponent>().GetComponentRepository();
            animationAtom.Start(entity);
        }

        public override TaskStatus OnUpdate()
        {
            var result = animationAtom.OnUpdate();
            if (result == RQ.AI.AtomActionResults.Success)
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Running;
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            animationAtom.End();
        }
    }
}
