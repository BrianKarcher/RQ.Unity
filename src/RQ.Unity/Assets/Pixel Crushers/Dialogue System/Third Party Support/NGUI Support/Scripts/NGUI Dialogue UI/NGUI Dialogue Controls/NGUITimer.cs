using UnityEngine;
using System;
using System.Collections;

namespace PixelCrushers.DialogueSystem.NGUISupport
{

    [AddComponentMenu("Pixel Crushers/Dialogue System/Third Party/NGUI/Timer")]
    public class NGUITimer : MonoBehaviour
    {

        public void StartCountdown(float duration, Action timeoutHandler)
        {
            StartCoroutine(Countdown(duration, timeoutHandler));
        }

        private IEnumerator Countdown(float duration, Action timeoutHandler)
        {
            UISlider uiSlider = GetComponent<UISlider>();
            if (uiSlider == null) yield break;
            float startTime = DialogueTime.time;
            float endTime = startTime + duration;
            while (DialogueTime.time < endTime)
            {
                float elapsed = DialogueTime.time - startTime;
                uiSlider.value = Mathf.Clamp(1 - (elapsed / duration), 0, 1);
                yield return null;
            }
            if (timeoutHandler != null) timeoutHandler();
        }

        public void OnDisable()
        {
            StopAllCoroutines();
        }

    }

}
