//using UnityEngine;
//using System.Collections;
//using RQ.Physics;
//using RQ.Entity.Common;
//using RQ.Entity.UI;
//using RQ.Animation;

//namespace RQ
//{
//    /// <summary>
//    /// A custom event that will load a level that is already setup in your build settings. 
//    /// </summary>
//    [WellFired.USequencerFriendlyName("Set Facing Direction")]
//    [WellFired.USequencerEvent("RQ/Sprite/Set Facing Direction")]
//    [WellFired.USequencerEventHideDuration()]
//    public class SetFacingDirection : WellFired.USEventBase 
//    {
//        private AnimationComponent _sprite2;
//        public Direction Direction;

//        public virtual void Start()
//        {
//            var _sprite2 = base.AffectedObject.GetComponent<AnimationComponent>();
//            //if (uiBase != null)
//            //    _sprite2 = uiBase.GetEntity() as IBaseGameEntity;
//            //if (_sprite2 == null)
//            //    _sprite2 = base.AffectedObject.GetComponentInChildren<SpriteManager>();
//        }
//        //public bool fireInEditor = false;
		
//        //public string Animation;
		
//        public override void FireEvent()
//        {
            
//            //_gameManagerScript = gameManager.GetComponent<GameManager>();
//            //_gameManagerScript.StartDialog (DialogName);

//            //_sprite2.SetAnimation(Animation);
//            _sprite2.SetFacingDirection(Direction);
//        }

//        public override void ProcessEvent(float deltaTime)
//        {

//        }
//    }
//}