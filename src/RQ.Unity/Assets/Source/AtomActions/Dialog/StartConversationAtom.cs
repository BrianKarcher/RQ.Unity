using RQ.AI;
using RQ.Entity.AtomAction;
using RQ.Entity.Components;
using RQ.Messaging;
using RQ.Model;
using RQ.Model.Interfaces;
using RQ.Physics.Components;
using System;
using UnityEngine;

namespace RQ.AI.Atom.UI
{
    [Serializable]
    public class StartConversationAtom : AtomActionBase
    {
        public string ComponentName;
        private ConversationTriggerComponent _conversationTriggerComponent;

        public override void Start(IComponentRepository entity)
        {
            base.Start(entity);
            _conversationTriggerComponent = entity.Components.GetComponent<ConversationTriggerComponent>(ComponentName);
            _conversationTriggerComponent.Trigger(entity.transform);
        }

        public override AtomActionResults OnUpdate()
        {
            return AtomActionResults.Success;
        }
    }
}
