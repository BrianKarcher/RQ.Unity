using BehaviorDesigner.Runtime.Tasks;
using RQ.AI.Drawers;
using RQ.Animation;
using RQ.Animation.BasicAction.Action;
using RQ.Animation.V2;
using RQ.Common;
using RQ.Entity.AtomAction.Action;
using RQ.Physics;
using RQ.Physics.Components;
using RQ.Physics.SteeringBehaviors;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Source.AI.Actions
{
    [TaskDescription("Entity idles. The task will return running until the task is done waiting. It will return success after the wait time has elapsed.")]
    [TaskCategory("RQ")]
    public class Idle : Wait
    {
        protected PhysicsComponent _physicsComponent;
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
            _physicsComponent = entity.Components.GetComponent<PhysicsComponent>();
            _physicsComponent.Stop();
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

        //public override TaskStatus OnUpdate()
        //{

        //}

        //public override void OnEnd()
        //{
        //    base.OnEnd();
        //    _steering.TurnOff(behavior_type.wander);
        //}   
        public override void OnEnd()
        {
            base.OnEnd();
            //animationAtom.End();
        }
    }
}
