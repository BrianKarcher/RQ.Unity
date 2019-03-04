using RQ;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.UI
{
    [AddComponentMenu("RQ/UI/Screen Resolution")]
    public class ScreenResolution : MonoBehaviour
    {
        public UILabel Resolution;
        public UIPopupList ResolutionPopup;
        public UIToggle FullScreenCheckbox;

        public void Start()
        {
            ResolutionPopup.itemData = new List<object>() { "0", "1" };
            int width = 1152;
            int height = 648;
            GameController.Instance.GetGraphicsEngine().SetScreenResolution(width, height, false);
        }

        public void ProcessNewResolution()
        {
            int width;
            int height;
            bool fullScreen;
            if (FullScreenCheckbox.value)
            {
                var highestRes = GameController.Instance.GetGraphicsEngine().GetHighestResolution();
                width = highestRes.width;
                height = highestRes.height;
                fullScreen = true;
            }
            else
            {
                
                //576x432
                //864x648
                if ((string)ResolutionPopup.data == "0")
                {
                    width = 576;
                    height = 432;
                }
                else
                {
                    width = 1152;
                    height = 648;
                }
                fullScreen = false;
            }
            GameController.Instance.GetGraphicsEngine().SetScreenResolution(width, height, fullScreen);
        }
    }
}
