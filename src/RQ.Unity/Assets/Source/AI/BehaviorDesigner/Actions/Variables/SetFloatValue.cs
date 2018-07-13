using BehaviorDesigner.Runtime.Tasks;
using RQ.Animation.BasicAction.Action;
using RQ.Physics.Components;

namespace RQ.Behavior.Actions
{
    [TaskDescription("Sets the float value.")]
    [TaskCategory("RQ")]
    public class SetFloatValue : Action
    {
        public SetPhysicsFloatValueAtom Atom;

        public override void OnStart()
        {
            base.OnStart();
            var entity = GetComponent<BehaviorTreeComponent>().GetComponentRepository();
            Atom.Start(entity);
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            Atom.End();
        }
    }
}
