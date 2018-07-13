using BehaviorDesigner.Runtime.Tasks;
using RQ.Animation.BasicAction.Action;
using RQ.Physics.Components;

namespace Assets.Source.AI.Actions
{
    [TaskDescription("The task will return running until the animation is complete. It will return success afterwards.")]
    [TaskCategory("RQ")]
    public class WaitForAnimationCompletion : Action
    {
        public WaitForAnimationCompletionAtom waitForAnimationCompletionAtom;

        public override void OnStart()
        {
            base.OnStart();
            var entity = GetComponent<BehaviorTreeComponent>().GetComponentRepository();
            waitForAnimationCompletionAtom.Start(entity);
            //_entity = GetComponent<BehaviorTreeComponent>().GetComponentRepository();
            ////_physicsComponent = entity.Components.GetComponent<PhysicsComponent>();
            //_animComponent = _entity.Components.GetComponent<AnimationComponent>();
            //_isRunning = true;
            //var spriteAnimator = _animComponent.GetSpriteAnimator() as SpriteAnimator;
            ////spriteAnimator.AnimComplete += SpriteAnimator_AnimComplete;
            ////if (!string.IsNullOrEmpty(Animation))
            ////    _animComponent.Animate(Animation);
            //StartListening(_entity);
        }

        //public void StartListening(IComponentRepository entity)
        //{
        //    //MessageDispatcher2.Instance.
        //}

        //public void StopListening(IComponentRepository entity)
        //{

        //}

        //private void SpriteAnimator_AnimComplete()
        //{
        //    _isRunning = false;
        //}

        public override TaskStatus OnUpdate()
        {
            return waitForAnimationCompletionAtom.OnUpdate() == RQ.AI.AtomActionResults.Success ? 
                TaskStatus.Success : TaskStatus.Running;
            //if (!_isRunning)
            //    return TaskStatus.Success;
            //else
            //    return TaskStatus.Running;
        }

        //public override TaskStatus OnUpdate()
        //{

        //}

        public override void OnEnd()
        {
            base.OnEnd();
            waitForAnimationCompletionAtom.End();
            //StopListening(_entity);
            //var spriteAnimator = _animComponent.GetSpriteAnimator() as SpriteAnimator;
            //spriteAnimator.AnimComplete -= SpriteAnimator_AnimComplete;
        }

        //public AnimationComponent GetAnimationComponent()
        //{
        //    var entity = Owner.GetComponent<BehaviorTreeComponent>().GetComponentRepository();
        //    var animComponent = entity.Components.GetComponent<AnimationComponent>();
        //    return animComponent;
        //}
    }
}
