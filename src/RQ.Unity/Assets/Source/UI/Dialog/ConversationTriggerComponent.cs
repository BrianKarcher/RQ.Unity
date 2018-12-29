using PixelCrushers.DialogueSystem;
using RQ.Common.Components;
using RQ.Common.Container;
using RQ.Entity.Common;
using RQ.Messaging;
using RQ.Model.Interfaces;
using RQ.Model.Serialization;
using RQ.Model.UI;
using System;
using UnityEngine;

namespace RQ.Physics.Components
{
    [AddComponentMenu("RQ/Components/UI/Conversation Trigger")]
    public class ConversationTriggerComponent : ComponentPersistence<ConversationTriggerComponent>,
        IEventTrigger
    {
        public event EventHandler EventEnded;

        public override void StartListening()
        {
            base.StartListening();
            MessageDispatcher2.Instance.StartListening("EndConversation", this.UniqueId, (data) =>
            {
                Debug.Log("Conversation Trigger EndConversation called");
                StopListening();
                if (EventEnded != null)
                    EventEnded.Invoke(this, EventArgs.Empty);
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher2.Instance.StopListening("EndConversation", this.UniqueId, -1);
        }

        public void Trigger(Transform requester)
        {
            StartListening();
            SendMessage("OnUse", requester, SendMessageOptions.DontRequireReceiver);
        }

        public override void Serialize(Serialization.EntitySerializedData entitySerializedData)
        {
            base.Serialize(entitySerializedData);
            var conversationTriggerData = new ConversationTriggerData();
            var conversationTrigger = GetComponent<ConversationTrigger>();
            if (conversationTrigger == null)
                return;
            if (conversationTrigger.actor != null)
            {
                var actorUniqueId = conversationTrigger.actor.GetComponent<SpriteBaseComponent>().UniqueId;
                conversationTriggerData.ActorUniqueId = actorUniqueId;
            }
            if (conversationTrigger.conversant != null)
            {
                var conversantUniqueId = conversationTrigger.conversant.GetComponent<SpriteBaseComponent>().UniqueId;
                conversationTriggerData.ConversantUniqueId = conversantUniqueId;
            }

            base.SerializeComponent(entitySerializedData, conversationTriggerData);
        }

        public override void Deserialize(Serialization.EntitySerializedData entitySerializedData)
        {
            base.Deserialize(entitySerializedData);
            var conversationTrigger = GetComponent<ConversationTrigger>();
            if (conversationTrigger == null)
                return;
            var conversationTriggerData = base.DeserializeComponent<ConversationTriggerData>(entitySerializedData);
            if (!string.IsNullOrEmpty(conversationTriggerData.ActorUniqueId))
            {
                var actor = EntityContainer._instance.GetEntity(conversationTriggerData.ActorUniqueId);
                conversationTrigger.actor = actor.transform;
            }
            if (!string.IsNullOrEmpty(conversationTriggerData.ConversantUniqueId))
            {
                var conversant = EntityContainer._instance.GetEntity(conversationTriggerData.ConversantUniqueId);
                conversationTrigger.conversant = conversant.transform;
            }
        }
    }
}
