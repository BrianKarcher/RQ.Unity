//using RQ.Animation;
//using RQ.Entity.Components;
//using RQ2.FSM.V2;
//using UnityEngine;
//using System.Linq;

//public class AnimateBehavior : StateMachineBehaviour
//{
//    [SerializeField]
//    private string _animation;

//    private AnimationComponent _animComponent;
//    private string _animUniqueId;
//    //private IComponentRepository _repo;

//    // Don't use Start in Assets
//    //public void Start()
//    //{
//    //    _animComponent = 
//    //}

//    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//    {
//        if (_animComponent == null)
//        {
//            var sm = animator.GetComponent<StateMachine2>();
//            var repo = sm.GetComponentRepository();
//            _animComponent = repo.Components.GetComponents<AnimationComponent>().FirstOrDefault(i => i.IsMain());
//        }
//        if (_animComponent == null)
//        {
//            Debug.LogError("Could not find Animation Component for " + animator.name);
//            return;
//        }
//        if (_animUniqueId == null)
//        {
//            _animUniqueId = _animComponent.GetUniqueIdForType(_animation);
//        }
//        _animComponent.Animate(_animUniqueId);
//    }
//}
