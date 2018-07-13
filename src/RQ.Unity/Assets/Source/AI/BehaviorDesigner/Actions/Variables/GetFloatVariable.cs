using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using RQ.Animation.BasicAction.Action;
using RQ.Physics.Components;
using UnityEngine;

namespace RQ.Behavior.Actions
{
    [TaskDescription("Gets the float variable.")]
    [TaskCategory("RQ")]
    public class GetFloatVariable : Action
    {

        public GetFloatVariableAtom Atom;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The value to set")]
        public SharedVariable fieldValue;

        public override void OnStart()
        {
            base.OnStart();
            var entity = GetComponent<BehaviorTreeComponent>().GetComponentRepository();
            Atom.Start(entity);
        }

        public override TaskStatus OnUpdate()
        {
            if (fieldValue == null)
            {
                Debug.LogWarning("Unable to get field - field value is null");
                return TaskStatus.Failure;
            }
            PerformGetValue();
            return TaskStatus.Success;
        }

        public void PerformGetValue()
        {
            Atom.OnUpdate();
            fieldValue.SetValue(Atom.Value);
        }

        public override void OnEnd()
        {
            base.OnEnd();
            Atom.End();
        }
    }
}
