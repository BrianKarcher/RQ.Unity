using UnityEngine;

namespace PixelCrushers.DialogueSystem.NGUI
{

    /// <summary>
    /// Adds fast forward control to continue buttons.
    /// </summary>
    [AddComponentMenu("Dialogue System/Third Party/NGUI/NGUI Continue Button Fast Forward")]
    public class NGUIContinueButtonFastForward : MonoBehaviour
    {

        public NGUIDialogueUI dialogueUI;

        public TypewriterEffect typewriterEffect;

        public void Awake()
        {
            if (dialogueUI == null)
            {
                dialogueUI = Tools.GetComponentAnywhere<NGUIDialogueUI>(gameObject);
            }
            if (typewriterEffect == null)
            {
                typewriterEffect = GetComponentInChildren<TypewriterEffect>();
            }
        }

        public void OnFastForward()
        {
            if ((typewriterEffect != null) && typewriterEffect.isActive)
            {
                typewriterEffect.Finish();
            }
            else
            {
                if (dialogueUI != null) dialogueUI.OnContinue();
            }
        }

    }

}