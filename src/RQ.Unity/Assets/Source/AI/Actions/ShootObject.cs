using BehaviorDesigner.Runtime.Tasks;
using RQ.AI.Action;
using RQ.AI.Drawers;
using RQ.Animation;
using RQ.Animation.BasicAction.Action;
using RQ.Animation.V2;
using RQ.Common;
using RQ.Physics;
using RQ.Physics.Components;
using RQ.Physics.SteeringBehaviors;
//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Source.AI.Actions
{
    [TaskDescription("Entity shoots a prefab. The task will return running until the task is done waiting. It will return success after the wait time has elapsed.")]
    [TaskCategory("RQ")]
    public class ShootObject : Action
    {
        //protected PhysicsComponent _physicsComponent;
        //private SteeringBehaviorManager _steering;
        //private AnimationComponent _animComponent;
        //[AIAnimationType]
        //public string Animation;
        public ShootObjectAtom shootObjectAtom;

        public override void OnStart()
        {
            base.OnStart();
            var entity = GetComponent<BehaviorTreeComponent>().GetComponentRepository();
            //_physicsComponent = entity.Components.GetComponent<PhysicsComponent>();
            //_animComponent = entity.Components.GetComponent<AnimationComponent>();
            //_steering = _physicsComponent.GetSteering();
            //_steering.TurnOn(behavior_type.wander);
            shootObjectAtom.Start(entity);
            //if (!string.IsNullOrEmpty(Animation))
            //    _animComponent.Animate(Animation);
        }

        //public override TaskStatus OnUpdate()
        //{

        //}

        public override void OnEnd()
        {
            base.OnEnd();
            //_steering.TurnOff(behavior_type.wander);
            shootObjectAtom.End();
        }

        public override void OnReset()
        {
            base.OnReset();
            shootObjectAtom = new ShootObjectAtom();
            shootObjectAtom._uniqueId = System.Guid.NewGuid().ToString();
        }

        //public AnimationComponent GetAnimationComponent()
        //{
        //    var entity = Owner.GetComponent<BehaviorTreeComponent>().GetComponentRepository();
        //    var animComponent = entity.Components.GetComponent<AnimationComponent>();
        //    return animComponent;
        //}
    }
}
