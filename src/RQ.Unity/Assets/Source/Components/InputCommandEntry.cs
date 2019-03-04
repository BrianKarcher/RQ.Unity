using RQ;
using RQ.Messaging;
using RQ.Model.Serialization.Input;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Source.Components
{
    [AddComponentMenu("RQ/UI/Input Command Entry")]
    public class InputCommandEntry : MessagingObject
    {
        public UILabel InputAction;
        public UILabel Countdown;
        //public UILabel InputTrigger;
        //public string ReplaceMessage;
        public string RemoveMessage;

        public InputCommand InputCommand { get; set; }

        private long _replaceClickMessageId;
        public int CountSeconds = 5;

        public override void OnEnable()
        {
            base.OnEnable();
            if (!Application.isPlaying)
                return;
            //if (GameStateController.Instance == null)
            //    return;
            InputCommand = GameStateController.Instance.CurrentSelectedInputCommand;
            //if (InputCommand == null)
            //    return;
            //string action;
            InputAction.text = String.Format("Press a key to assign it to {0}.", InputCommand.InputAction.ToFriendlyName());
            //var triggerText = InputCommand.IsAxis ?
            //    InputCommand.AxisName :
            //    InputCommand.KeyCode.ToString();
            //InputTrigger.text = triggerText;
            //_currentCount = 5;
            StartCoroutine(WaitForKeyPress());
        }

        IEnumerator WaitForKeyPress()
        {
            for (int i = 0; i < CountSeconds; i++)
            {
                Countdown.text = i.ToString();
                yield return new WaitForSeconds(1f);
            }
            MessageDispatcher2.Instance.DispatchMsg("Cancel", 0f, this.UniqueId, "UI Manager", null);
            yield return null;
        }

        public override void Update()
        {
            base.Update();
            // Is a key pressed?
            System.Array values = System.Enum.GetValues(typeof(KeyCode));
            foreach (KeyCode code in values)
            {
                if (Input.GetKeyDown(code))
                {
                    print(System.Enum.GetName(typeof(KeyCode), code));
                    InputCommand.IsAxis = false;
                    InputCommand.KeyCode = code;
                    Done();
                }
            }

            // Check the axis

        }

        private void Done()
        {
            MessageDispatcher2.Instance.DispatchMsg("Done", 0f, this.UniqueId, "UI Manager", null);
        }

        //public override void StartListening()
        //{
        //    base.StartListening();
        //    _replaceClickMessageId = MessageDispatcher2.Instance.StartListening("ButtonClicked", this.UniqueId, (data) =>
        //    {
        //        //if ((string)data.ExtraInfo == "Replace")
        //        //{

        //        //}
        //        if ((string)data.ExtraInfo == "Remove")
        //        {
        //            InputCommand.ClearKeys();
        //            MessageDispatcher2.Instance.DispatchMsg(RemoveMessage, 0f, this.UniqueId, "UI Manager", null);
        //        }
        //    });
        //}

        //public override void StopListening()
        //{
        //    base.StopListening();
        //    MessageDispatcher2.Instance.StopListening("ButtonClicked", this.UniqueId, _replaceClickMessageId);
        //}
    }
}
