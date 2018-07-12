using BehaviorDesigner.Runtime.Tasks;
using RQ.Animation.BasicAction.Action;
using RQ.Physics.Components;

namespace Assets.Source.AI.Actions
{
    [TaskDescription("Sends a message.")]
    [TaskCategory("RQ")]
    public class SendMessage : Action
    {
        public SendMessageAtom Atom;

        public override void OnStart()
        {
            base.OnStart();
            var entity = GetComponent<BehaviorTreeComponent>().GetComponentRepository();
            Atom.Start(entity);
        }

        public override TaskStatus OnUpdate()
        {
            var result = Atom.OnUpdate();
            if (Atom.IsFinished())
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
            Atom.End();
        }
    }
}
