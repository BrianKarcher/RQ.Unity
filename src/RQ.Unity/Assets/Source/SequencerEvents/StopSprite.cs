//using UnityEngine;
//using System.Collections;
//using RQ.Physics;
//using RQ.Entity.Common;
//using RQ.Entity.UI;
//using RQ.Physics.Components;
//namespace RQ
//{
//    /// <summary>
//    /// A custom event that will stop a Kinetic object
//    /// </summary>
//    [WellFired.USequencerFriendlyName("Stop Sprite")]
//    [WellFired.USequencerEvent("RQ/Sprite/Stop Sprite")]
//    [WellFired.USequencerEventHideDuration()]
//    public class StopSprite : WellFired.USEventBase
//    {
//        private PhysicsComponent _sprite2;
//        public Direction Direction;

//        public virtual void Start()
//        {
//            var _sprite2 = base.AffectedObject.GetComponent<PhysicsComponent>();
//        }
//        //public bool fireInEditor = false;
		
//        //public string Animation;
		
//        public override void FireEvent()
//        {
            
//            //_gameManagerScript = gameManager.GetComponent<GameManager>();
//            //_gameManagerScript.StartDialog (DialogName);

//            //_sprite2.SetAnimation(Animation);
//            //_sprite2.SetDirection(Direction);
//            //_sprite2.Stop();
//        }

//        public override void ProcessEvent(float deltaTime)
//        {

//        }
//    }
//}