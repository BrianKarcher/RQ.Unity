using PixelCrushers.DialogueSystem;
using RQ.Common.Controllers;
using RQ.Logging;
using RQ.Messaging;
using System;
using UnityEngine;

namespace RQ.UI
{

    /// <summary>
    /// Links the conversation start and end to the UI Manager
    /// </summary>
    [AddComponentMenu("Dialogue System/Actor/Conversation Link")]
    public class ConversationLink : MonoBehaviour
    {
        private bool _hasStarted = false;

        public event Action ConversationEnd;
        //public UIManager UIManager;
        //private UIManager _uiManager;

        //public void Start()
        //{
        //    if (UIManager != null)
        //        _uiManager = UIManager.GetComponent<UIManager>();
        //}
        //public UIManager 
        public void Start()
        {
            _hasStarted = true;
        }
        public void OnConversationStart(Transform actor)
        {
            if (!_hasStarted)
                return;

            Debug.LogError("(Conversation Link) OnConversationStart called");
            //MessageDispatcher.Instance.DispatchMsg(0f, "UI Manager", "UI Manager", Enums.Telegrams.ConversationBegin,
            //    actor);
            //MessageDispatcher.Instance.DispatchMsg(0f, string.Empty, "Game Controller", Enums.Telegrams.Pause,
            //    actor);
            // If a conversation is starting we need to get out of the Start Menu if we are there
            MessageDispatcher2.Instance.DispatchMsg("Playtime", 0f, string.Empty, "UI Manager", null);
            MessageDispatcher2.Instance.DispatchMsg("StartConversation", 0f, string.Empty, "Game Controller", null);
            GameController.Instance.UIManager.HoverContinueButton();
            //var continueButton = GameController.Instance.UIManager.ContinueButton;
            //UICamera.currentScheme = UICamera.ControlScheme.Controller;
            //UICamera.hoveredObject = continueButton.gameObject;
            //UIManager.OnConversationStart(actor);
        }

        public void OnConversationEnd(Transform actor)
        {
            if (!_hasStarted)
                return;
            //Debug.Log("OnConversationEnd called");
            //UIManager.OnConversationEnd(actor);
            //MessageDispatcher.Instance.DispatchMsg(0f, "UI Manager", "UI Manager", Enums.Telegrams.ConversationEnd,
            //    actor);
            //MessageDispatcher.Instance.DispatchMsg(0f, string.Empty, "Game Controller", Enums.Telegrams.Unpause,
            //    actor);
            MessageDispatcher2.Instance.DispatchMsg("EndConversation", 0f, string.Empty, "Game Controller", null);
            if (GameDataController.Instance.LastUsedEntity != null)
                MessageDispatcher2.Instance.DispatchMsg("SetGameProgress", 0f, string.Empty, GameDataController.Instance.LastUsedEntity.UniqueId, null);
            GameDataController.Instance.LastUsedEntity = null;
            if (ConversationEnd != null)
                ConversationEnd();
        }

        public void OnConversationLine(Subtitle subtitle)
        {
            if (!_hasStarted)
                return;
        }
    }
}
