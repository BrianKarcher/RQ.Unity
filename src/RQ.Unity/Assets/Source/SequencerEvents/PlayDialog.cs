//using UnityEngine;
//using System.Collections;
//namespace RQ
//{
//    /// <summary>
//    /// A custom event that will load a level that is already setup in your build settings. 
//    /// </summary>
//    [WellFired.USequencerFriendlyName("Play Dialog")]
//    [WellFired.USequencerEvent("RQ/Dialog/PlayDialog")]
//    [WellFired.USequencerEventHideDuration()]
//    public class PlayDialog : WellFired.USEventBase 
//    {
//        //public bool fireInEditor = false;
		
//        public string DialogName;
//        public Transform gameManager;
//        //private GameManager _gameManagerScript;
		
//        public override void FireEvent()
//        {
//            // @todo implement this with the new dialog system
//            //_gameManagerScript = gameManager.GetComponent<GameManager>();
//            //_gameManagerScript.StartDialog (DialogName);
//        }

//        public override void ProcessEvent(float deltaTime)
//        {

//        }
//    }
//}