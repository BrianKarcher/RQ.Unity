using BehaviorDesigner.Runtime.Tasks;
using RQ.AI.Conditions;
using RQ.Entity.Common;
using RQ.Messaging;
using RQ.Physics;
using RQ.Physics.Components;
using System;

namespace RQ.AI.Conditionals
{
    [TaskDescription("Returns Success if the entity has been damaged. Failure otherwise.")]
    [TaskCategory("RQ")]
    public class IsDamaged : Conditional
    {
        private IsDamagedConditionAI _isDamagedAI;

        public override void OnStart()
        {
            base.OnStart();
            var entity = GetComponent<BehaviorTreeComponent>().GetComponentRepository();
			_isDamagedAI = new IsDamagedConditionAI();
            _isDamagedAI.Start(entity);
        }

        public override void OnEnd()
        {
            base.OnEnd();
        }

        public override TaskStatus OnUpdate()
        {
            if (_isDamagedAI.OnUpdate() == AtomActionResults.Success)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}
