using BehaviorDesigner.Runtime.Tasks;
using RQ.Physics;

namespace RQ.AI.Conditionals
{
    [TaskDescription("Returns Success if the entity is within the camera viewport. Failure otherwise.")]
    [TaskCategory("RQ")]
    public class EntityInViewport : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            var camera = GameController.Instance.GetCamera();
            Vector2D pos = transform.position; //physicsComponent.GetPos();
            var isInViewport = camera.IsPosInViewport(pos);
            if (isInViewport)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}
