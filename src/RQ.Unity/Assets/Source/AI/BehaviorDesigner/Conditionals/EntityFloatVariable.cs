using BehaviorDesigner.Runtime.Tasks;
using RQ.FSM.V2.Conditionals;
using RQ.Physics.Components;
using UnityEngine;

namespace RQ.AI.Conditionals
{
    [TaskDescription("Returns Success if the entity is within the camera viewport. Failure otherwise.")]
    [TaskCategory("RQ")]
    public class EntityFloatVariable : Conditional
    {
        protected AltitudePhysicsComponent _altitudePhysicsComponent;
        protected PhysicsComponent _physicsComponent;
        protected EntityStatsComponent _entityStatsComponent;
        [SerializeField]
        private Operator _operator = Operator.Equal;
        [SerializeField]
        private FloatVariableEnum _variable = FloatVariableEnum.Altitude;
        [SerializeField]
        private float _value = 0f;

        public override void OnStart()
        {
            base.OnStart();
            var entity = GetComponent<BehaviorTreeComponent>().GetComponentRepository();
            _physicsComponent = entity.Components.GetComponent<PhysicsComponent>();
            _altitudePhysicsComponent = entity.Components.GetComponent<AltitudePhysicsComponent>();
            _entityStatsComponent = entity.Components.GetComponent<EntityStatsComponent>();
        }

        public override TaskStatus OnUpdate()
        {
            var value = GetValue();
            var testResult = Test(value);

            if (testResult)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        private float GetValue()
        {
            switch (_variable)
            {
                case FloatVariableEnum.Altitude:
                    return _altitudePhysicsComponent.GetAltitudeData().Altitude.y;
                case FloatVariableEnum.HP:
                    return _entityStatsComponent.GetEntityStats().CurrentHP;
                case FloatVariableEnum.InputForce:
                    return _physicsComponent.GetPhysicsData().InputForce.Length();
            }
            return 0f;
        }

        private bool Test(float value)
        {
            switch (_operator)
            {
                case Operator.Equal:
                    return value == _value;
                case Operator.GreaterThen:
                    return value > _value;
                case Operator.GreaterThenOrEqualTo:
                    return value >= _value;
                case Operator.LessThanOrEqualTo:
                    return value <= _value;
                case Operator.LessThen:
                    return value < _value;
                case Operator.NotEqual:
                    return value != _value;
                case Operator.Between:
                    return value >= _value && value <= _value;
            }
            return false;
        }
    }
}
