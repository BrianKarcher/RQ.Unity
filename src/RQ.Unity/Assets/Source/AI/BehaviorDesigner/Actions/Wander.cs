using BehaviorDesigner.Runtime.Tasks;
using RQ.Physics;
using RQ.Physics.Components;
using RQ.Physics.SteeringBehaviors;

namespace Assets.Source.AI.Actions
{
    [TaskDescription("Entity wanders aimlessly. The task will return running until the task is done waiting. It will return success after the wait time has elapsed.")]
    [TaskCategory("RQ")]
    public class Wander : Wait
    {
        protected PhysicsComponent _physicsComponent;
        private SteeringBehaviorManager _steering;
        //private AnimationComponent _animComponent;
        //[AIAnimationType]
        //public string Animation;
        //public AnimationAtom animationAtom;

        public override void OnStart()
        {
            base.OnStart();
            var entity = GetComponent<BehaviorTreeComponent>().GetComponentRepository();
            _physicsComponent = entity.Components.GetComponent<PhysicsComponent>();
            //_animComponent = entity.Components.GetComponent<AnimationComponent>();
            _steering = _physicsComponent.GetSteering() as SteeringBehaviorManager;
            _steering.TurnOn(behavior_type.wander);
            //animationAtom.Start(entity);
            //if (!string.IsNullOrEmpty(Animation))
            //    _animComponent.Animate(Animation);
        }

        //public override TaskStatus OnUpdate()
        //{

        //}

        public override void OnEnd()
        {
            base.OnEnd();
            _steering.TurnOff(behavior_type.wander);
            //animationAtom.End();
        }

        //public AnimationComponent GetAnimationComponent()
        //{
        //    var entity = Owner.GetComponent<BehaviorTreeComponent>().GetComponentRepository();
        //    var animComponent = entity.Components.GetComponent<AnimationComponent>();
        //    return animComponent;
        //}
    }
}
