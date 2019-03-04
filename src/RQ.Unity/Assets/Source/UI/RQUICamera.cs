using RQ.Common.UI;
using UnityEngine;

namespace RQ2.UI
{
    [AddComponentMenu("RQ/UI/UI Camera")]
    public class RQUICamera : MonoBehaviour, IUICamera
    {
        [SerializeField]
        private UICamera _camera;

        public void SetKeys(KeyCode submitKey0, KeyCode submitKey1, KeyCode cancelKey0, KeyCode cancelKey1)
        {
            _camera.submitKey0 = KeyCode.LeftControl;
            _camera.submitKey1 = KeyCode.Joystick1Button0;
            _camera.cancelKey0 = KeyCode.Escape;
            _camera.cancelKey1 = KeyCode.Joystick1Button1;
        }
    }
}
