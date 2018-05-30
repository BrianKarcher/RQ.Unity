using BehaviorDesigner.Runtime.Tasks;
using RQ.AI.Drawers;
using RQ.Animation;
using RQ.Animation.BasicAction.Action;
using RQ.Animation.V2;
using RQ.Common;
using RQ.Physics;
using RQ.Physics.Components;
using RQ.Physics.SteeringBehaviors;
using RQ2.AnimationV2;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Source.AI.Actions
{
    [TaskDescription("Entity wanders aimlessly. The task will return running until the task is done waiting. It will return success after the wait time has elapsed.")]
    [TaskCategory("RQ")]
    public class PlayAnimation : Action, IAnimatedObject
    {
        //protected PhysicsComponent _physicsComponent;
        //private SteeringBehaviorManager _steering;
        private AnimationComponent _animComponent;
        //[AIAnimationType]
        //public string Animation;
        public AnimationAtom animationAtom;
        public bool ExitImmediately;
        private bool _isRunning;

        public override void OnStart()
        {
            base.OnStart();
            var entity = GetComponent<BehaviorTreeComponent>().GetComponentRepository();
            //_physicsComponent = entity.Components.GetComponent<PhysicsComponent>();
            _animComponent = entity.Components.GetComponent<AnimationComponent>();
            //_steering = _physicsComponent.GetSteering();
            //_steering.TurnOn(behavior_type.wander);
            animationAtom.Start(entity);
            if (ExitImmediately)
                _isRunning = false;
            else
                _isRunning = true;
            var spriteAnimator = _animComponent.GetSpriteAnimator() as SpriteAnimator;
            spriteAnimator.AnimComplete += SpriteAnimator_AnimComplete;
            //if (!string.IsNullOrEmpty(Animation))
            //    _animComponent.Animate(Animation);
        }

        private void SpriteAnimator_AnimComplete()
        {
            _isRunning = false;
        }

        public override TaskStatus OnUpdate()
        {
            if (!_isRunning)
                return TaskStatus.Success;
            else
                return TaskStatus.Running;
        }

        //public override TaskStatus OnUpdate()
        //{

        //}

        public override void OnEnd()
        {
            base.OnEnd();
            //_steering.TurnOff(behavior_type.wander);
            animationAtom.End();
            var spriteAnimator = _animComponent.GetSpriteAnimator() as SpriteAnimator;
            spriteAnimator.AnimComplete -= SpriteAnimator_AnimComplete;
        }

        public AnimationComponent GetAnimationComponent()
        {
            var entity = Owner.GetComponent<BehaviorTreeComponent>().GetComponentRepository();
            var animComponent = entity.Components.GetComponent<AnimationComponent>();
            return animComponent;
        }

        //public override void OnReset()
        //{
        //    base.OnReset();
        //    if (animationAtom.)
        //}
    }
}
