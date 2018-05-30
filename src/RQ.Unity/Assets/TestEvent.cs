using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;
using WellFired;
namespace RQ
{
    /// <summary>
    /// A custom event that will load a level that is already setup in your build settings. 
    /// </summary>
    [WellFired.USequencerFriendlyName("Test Event")]
    [WellFired.USequencerEvent("RQ/Dialog/TestEvent")]
    [WellFired.USequencerEventHideDuration()]
    public class TestEvent : USEventBase
    {
        //public bool fireInEditor = false;

        //public string DialogName;
        //public Transform gameManager;
        //private GameManager _gameManagerScript;

        public override void FireEvent()
        {
            ConversationTrigger trigger = AffectedObject.GetComponent<ConversationTrigger>();
            // @todo implement this with the new dialog system
            //_gameManagerScript = gameManager.GetComponent<GameManager>();
            //_gameManagerScript.StartDialog (DialogName);
            trigger.TryStartConversation(trigger.actor);
        }

        public override void ProcessEvent(float deltaTime)
        {

        }
    }
}