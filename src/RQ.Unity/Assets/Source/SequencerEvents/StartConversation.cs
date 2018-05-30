using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;
using WellFired;
using RQ.Logging;
using RQ.Messaging;
namespace RQ.SequencerEvents
{
    /// <summary>
    /// A custom event that will load a level that is already setup in your build settings. 
    /// </summary>
    [WellFired.USequencerFriendlyName("Start Conversation")]
    [WellFired.USequencerEvent("RQ/Dialog/Start Conversation")]
    [WellFired.USequencerEventHideDuration()]
    public class StartConversation : USEventBase
    {
        [SerializeField]
        private int _conversationIndex;
        //public void Awake()
        //{
        //    Log.Info("StartConversation Awaked");
        //}
        //public bool fireInEditor = false;

        //public string DialogName;
        //public Transform gameManager;
        //private GameManager _gameManagerScript;

        public override void FireEvent()
        {
            //Log.Info("StartConversation event fired in " + this.name + ".");
            ConversationTrigger trigger = AffectedObject.GetComponent<ConversationTrigger>();
            // @todo implement this with the new dialog system
            //_gameManagerScript = gameManager.GetComponent<GameManager>();
            //_gameManagerScript.StartDialog (DialogName);
            //var sequence = base.Sequence;
            //MessageDispatcher.Instance.DispatchMsg(0f, string.Empty, "UI Manager", 
            //    Enums.Telegrams.SetStartSequenceOnConversationEnd, sequence);
            MessageDispatcher2.Instance.DispatchMsg("SetConversationIndex", 0f, string.Empty,
                "UI Manager", _conversationIndex);
            //sequence.Pause();
            DialogueManager.StartConversation(trigger.conversation);
            trigger.useConversationTitlePicker = true;
            //trigger.TryStartConversation(trigger.actor);
        }

        public override void ProcessEvent(float deltaTime)
        {

        }
    }
}