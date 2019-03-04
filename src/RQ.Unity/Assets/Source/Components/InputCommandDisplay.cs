using RQ;
using RQ.Messaging;
using RQ.Model.Serialization.Input;
using UnityEngine;

namespace Assets.Source.Components
{
    [AddComponentMenu("RQ/UI/Input Command Display")]
    public class InputCommandDisplay : MessagingObject
    {
        public UILabel InputAction;
        public UILabel InputTrigger;
        //public string ReplaceMessage;
        public string RemoveMessage;

        public InputCommand InputCommand { get; set; }

        private long _replaceClickMessageId;

        public override void OnEnable()
        {
            base.OnEnable();
            if (GameStateController.Instance == null)
                return;
            InputCommand = GameStateController.Instance.CurrentSelectedInputCommand;
            if (InputCommand == null)
                return;
            InputAction.text = InputCommand.InputAction.ToFriendlyName();
            var triggerText = InputCommand.IsAxis ?
                InputCommand.AxisName :
                InputCommand.KeyCode.ToString();
            InputTrigger.text = triggerText;
        }

        public override void StartListening()
        {
            base.StartListening();
            _replaceClickMessageId = MessageDispatcher2.Instance.StartListening("ButtonClicked", this.UniqueId, (data) =>
            {
                //if ((string)data.ExtraInfo == "Replace")
                //{

                //}
                if ((string)data.ExtraInfo == "Remove")
                {
                    InputCommand.ClearKeys();
                    MessageDispatcher2.Instance.DispatchMsg(RemoveMessage, 0f, this.UniqueId, "UI Manager", null);
                }
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher2.Instance.StopListening("ButtonClicked", this.UniqueId, _replaceClickMessageId);
        }
    }
}
