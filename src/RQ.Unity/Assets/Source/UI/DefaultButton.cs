using UnityEngine;

namespace Assets.Source.UI
{
    [AddComponentMenu("RQ/UI/Default Button")]
    public class DefaultButton : MonoBehaviour
    {
        public UIButton DefaultBtn;

        public void OnEnable()
        {
            UICamera.hoveredObject = DefaultBtn.gameObject;
        }
    }
}
