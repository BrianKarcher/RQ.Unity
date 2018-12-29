using PixelCrushers.DialogueSystem;
using RQ.AI.Atom.UI;
using UnityEngine;

namespace RQ.Controller.Actions
{
    [AddComponentMenu("RQ/Action/UI/Start Conversation")]
    public class StartConversation : ActionBase
    {
        [SerializeField]
        //[Obsolete]
        private ConversationTrigger _trigger;
        public StartConversationAtom _atom;
        //[SerializeField]
        //private SpriteBaseComponent _conversationTrigger;

        public override void Act(Component otherRigidBody)
        {
            base.Act(otherRigidBody);
            _atom.Start(GetEntity());
            //if (_trigger == null)
            //{
            //    _trigger = _conversationTrigger.GetComponent<ConversationTrigger>();
            //}
            //DialogueManager.StartConversation(_trigger.conversation);
            //_trigger.useConversationTitlePicker = true;
            //GameController._instance.AudioSource.PlayOneShot(AudioClip);
        }

        //public override void Serialize(EntitySerializedData entityData)
        //{
        //    base.Serialize(entityData);
        //    var conversationUniqueId = new SimpleStringData() { Value = _conversationTrigger.UniqueId };
        //    base.SerializeComponent(entityData, conversationUniqueId);
        //    //var conversationTriggerData
        //    //var conversationTriggers = GetComponents<ConversationTrigger>();
        //    //foreach (var conversationTrigger in conversationTriggers)
        //    //{

        //    //}
        //}

        //public override void Deserialize(EntitySerializedData entityData)
        //{
        //    base.Deserialize(entityData);
        //    // TODO NOW! Make the generic an object instead of a string!!!!!!!!!!!
        //    // Cannot deserialize a string like this!!!!!!!
        //    var conversationTriggerUniqueId = base.DeserializeComponent<SimpleStringData>(entityData);
        //    if (conversationTriggerUniqueId != null && !string.IsNullOrEmpty(conversationTriggerUniqueId.Value))
        //    {
        //        _conversationTrigger = EntityContainer._instance.GetEntity(conversationTriggerUniqueId.Value).transform.GetComponent<SpriteBaseComponent>();
        //    }
        //}
    }
}
