using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelCrushers.DialogueSystem.NGUI
{

    /// <summary>
    /// This script adds a key or button trigger to a an NGUI UIButton.
    /// Contributed by the Binding Force devs.
    /// </summary>
    [AddComponentMenu("Dialogue System/UI/NGUI/Effects/NGUI Button Key Trigger")]
    public class NGUIButtonKeyTrigger : MonoBehaviour
    {

        public KeyCode key = KeyCode.None;
        public string buttonName = string.Empty;
        private GameObject target = null;

        public UIButton button = null;

        void Awake()
        {
            button = GetComponent<UIButton>();
            if (button == null) { enabled = false; } else
            {
                target = button.gameObject;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(key) || (!string.IsNullOrEmpty(buttonName) && Input.GetButtonDown(buttonName)))
            {
                var pointer = new PointerEventData(EventSystem.current);
                ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.submitHandler);
            }
        }

        public Response response { get; set; }

        public void OnGUI()
        {
            if (Input.GetKeyDown(key) || (!string.IsNullOrEmpty(buttonName) && Input.GetButtonDown(buttonName)))
            {
                button.state = UIButtonColor.State.Pressed;
                Debug.Log("Pressing " + buttonName);
            }
            if (Input.GetKeyUp(key) || (!string.IsNullOrEmpty(buttonName) && Input.GetButtonUp(buttonName)))
            {
                if (target != null) target.SendMessage("OnClick", response, SendMessageOptions.DontRequireReceiver);
                Debug.Log("Clicked " + buttonName);
            }
            
        }

    }

}
