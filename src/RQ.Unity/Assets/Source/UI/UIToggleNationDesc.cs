using RQ2.Editor.UI;
using UnityEngine;

namespace RQ2.Controller.UI
{
    [ExecuteInEditMode]
    [AddComponentMenu("RQ/UI/Toggle Nation Desc")]
    public class UIToggleNationDesc : MonoBehaviour
    {
        //[SerializeField]
        public UINationDescConfig NationDesc;
        public UISprite Banner;
        public UILabel NationLabel;
        public UILabel NationDescLabel;

        public void Awake()
        {
            if (!Application.isPlaying)
                return;
            UIToggle toggle = GetComponent<UIToggle>();
            EventDelegate.Add(toggle.onChange, Toggle);
        }

        public void Toggle()
        {
            Banner.spriteName = NationDesc.NationBanner;
            NationLabel.text = NationDesc.Nation;
            NationDescLabel.text = NationDesc.NationDesc;
        }
    }
}
