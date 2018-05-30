//using UnityEngine;
//using System.Collections;
//using RQ.Physics;
//using RQ.AI;
//using RQ.Entity.UI;
//using RQ.Entity.Common;
//using RQ.Animation;
//namespace RQ
//{
//    /// <summary>
//    /// A custom event that will load a level that is already setup in your build settings. 
//    /// </summary>
//    [WellFired.USequencerFriendlyName("Play Animation Set Dir")]
//    [WellFired.USequencerEvent("RQ/Sprite/PlayAnimationSetDir")]
//    [WellFired.USequencerEventHideDuration()]
//    public class PlayAnimationSetDir : WellFired.USEventBase 
//    {
//        private AnimationComponent _sprite2;

//        //public virtual void Start()
//        //{
            
//        //}
//        //public bool fireInEditor = false;
		
//        public string Animation;
//        public Direction Direction;
		
//        public override void FireEvent()
//        {
//            var _sprite2 = base.AffectedObject.GetComponent<AnimationComponent>();
//            //if (uiBase != null)
//            //    _sprite2 = uiBase.GetEntity() as IBaseGameEntity;
//            //_gameManagerScript = gameManager.GetComponent<GameManager>();
//            //_gameManagerScript.StartDialog (DialogName);
//            _sprite2.SetFacingDirection(Direction);
//            _sprite2.GetSpriteAnimator().Render(Animation);
//        }

//        public override void ProcessEvent(float deltaTime)
//        {

//        }
//    }
//}