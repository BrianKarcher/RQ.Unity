using UnityEngine;

namespace PixelCrushers.DialogueSystem.NGUISupport
{

    /// <summary>
    /// This works just like NGUIDialogueUI except it uses the speaker's bark UI
    /// instead of the dialogue UI labels.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Dialogue System/Third Party/NGUI/NGUI Dialogue Bark UI")]
    public class NGUIDialogueBarkUI : NGUIDialogueUI
    {

        public override void ShowSubtitle(Subtitle subtitle)
        {
            StartCoroutine(BarkController.Bark(subtitle));
        }

    }

}
