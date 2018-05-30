using RQ.Model;
using RQ.Render.UI;
using UnityEngine;

namespace RQ2.UI
{
    [AddComponentMenu("RQ/UI/Overlay Window")]
    public class OverlayWindow : MonoBehaviour, IOverlayWindow
    {
        [SerializeField]
        private UISprite _overlayColorWindow = null;
        private TweenColor _currentTween;

        public void TweenOverlayToColor(TweenToColorInfo colorInfo)
        {
            //Debug.LogWarning("Tweening overlay to " + colorInfo.Color.a);
            //_overlayColorWindow.color = new Color(0, 0, 0, 1);
            _currentTween = TweenColor.Begin(_overlayColorWindow.gameObject, colorInfo.Duration,
                colorInfo.Color);
            //var tween = TweenAlpha.Begin(FadeWindow.gameObject, 20f, 0);
            _currentTween.delay = colorInfo.Delay;
            //tween.to = Color.white;
        }

        public void SetOverlayToColor(Color color)
        {
            //Debug.LogWarning("Tweening overlay to " + color.a);
            _overlayColorWindow.color = color;
        }

        public Color GetOverlayColor()
        {
            return _currentTween.value;
            //return _overlayColorWindow.value;
        }
    }
}
