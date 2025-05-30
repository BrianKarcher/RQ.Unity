﻿using BehaviorDesigner.Runtime.Tasks;
using RQ.AI.Action;
using RQ.Physics.Components;

namespace Assets.Source.AI.Actions
{
    [TaskDescription("Entity stops moving. The task will return success.")]
    [TaskCategory("RQ")]
    public class Stop : Action
    {
        protected StopMovingAtom _stopMovingAtom;
        //private AnimationComponent _animComponent;
        //public AnimationAtom animationAtom;
        //public PlaySoundOneShotAtom PlaySoundOneShotAtom;
        //[AIAnimationType]
        //public string Animation;
        //private SteeringBehaviorManager _steering;

        public override void OnStart()
        {
            base.OnStart();
            var entity = GetComponent<BehaviorTreeComponent>().GetComponentRepository();
            _stopMovingAtom = new StopMovingAtom();
            _stopMovingAtom.Start(entity);
            //_physicsComponent = entity.Components.GetComponent<PhysicsComponent>();
            //_physicsComponent.Stop();
            //animationAtom.Start(entity);
            //_animComponent = entity.Components.GetComponent<AnimationComponent>();
            //if (!string.IsNullOrEmpty(Animation))
            //    _animComponent.Animate(Animation);
            //_steering = _physicsComponent.GetSteering();
            //_steering.TurnOn(behavior_type.wander);
        }

        //public AnimationComponent GetAnimationComponent()
        //{            
        //    var entity = Owner.GetComponent<BehaviorTreeComponent>().GetComponentRepository();
        //    var animComponent = entity.Components.GetComponent<AnimationComponent>();
        //    return animComponent;
        //}

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }

        //public override void OnEnd()
        //{
        //    base.OnEnd();
        //    _steering.TurnOff(behavior_type.wander);
        //}   
        public override void OnEnd()
        {
            base.OnEnd();
            _stopMovingAtom.End();
            //animationAtom.End();
        }
    }
}
