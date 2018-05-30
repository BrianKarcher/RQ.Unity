//using UnityEngine;
//using System.Collections;
//using RQ.Physics;
//using RQ.Entity.Common;
//using RQ.Entity.UI;
//namespace RQ
//{
//    /// <summary>
//    /// A custom event that will load a level that is already setup in your build settings. 
//    /// </summary>
//    [WellFired.USequencerFriendlyName("Record Position")]
//    [WellFired.USequencerEvent("RQ/Sprite/Record Position")]
//    [WellFired.USequencerEventHideDuration()]
//    public class RecordPosition : WellFired.USEventBase 
//    {
//        private Transform _sprite2;

//        public virtual void Start()
//        {
//            var uiBase = base.AffectedObject.transform;
//            if (uiBase != null)
//                _sprite2 = uiBase;
//        }
//        //public bool fireInEditor = false;
		
//        //public string Animation;
		
//        public override void FireEvent()
//        {
            
//            //_gameManagerScript = gameManager.GetComponent<GameManager>();
//            //_gameManagerScript.StartDialog (DialogName);

//            //_sprite2.SetAnimation(Animation);
//            //_sprite2.RecordPosition();
//        }

//        public override void ProcessEvent(float deltaTime)
//        {

//        }
//    }
//}