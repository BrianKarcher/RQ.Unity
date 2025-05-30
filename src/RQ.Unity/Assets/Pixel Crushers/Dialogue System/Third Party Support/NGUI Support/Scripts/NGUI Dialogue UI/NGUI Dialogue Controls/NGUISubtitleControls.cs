using UnityEngine;
using System.Collections;

namespace PixelCrushers.DialogueSystem.NGUISupport
{

    /// <summary>
    /// Subtitle NGUI controls for NGUIDialogueUI.
    /// </summary>
    [System.Serializable]
    public class NGUISubtitleControls : AbstractUISubtitleControls
    {

        /// <summary>
        /// The panel containing the response menu controls. A panel is optional, but you may want one
        /// so you can include a background image, panel-wide effects, etc.
        /// </summary>
        public global::UIPanel panel;

        /// <summary>
        /// The label that will show the text of the subtitle.
        /// </summary>
        public UILabel line;

        /// <summary>
        /// The label that will show the portrait image.
        /// </summary>
        public UITexture portraitImage;

        /// <summary>
        /// The label that will show the name of the speaker.
        /// </summary>
        public UILabel portraitName;

        /// <summary>
        /// The continue button. This is only required if DisplaySettings.waitForContinueButton 
        /// is <c>true</c> -- in which case this button should send "OnContinue" to the UI when clicked.
        /// </summary>
        public UIButton continueButton;

        [HideInInspector]
        public Color originalColor = Color.clear;

        /// <summary>
        /// Indicates whether this subtitle is currently assigned text.
        /// </summary>
        /// <value>
        /// <c>true</c> if it has text; otherwise, <c>false</c>.
        /// </value>
        public override bool hasText
        {
            get { return (line != null) && !string.IsNullOrEmpty(line.text); }
        }

        /// <summary>
        /// Sets the subtitle controls active or inactive.
        /// </summary>
        /// <param name='value'>
        /// <c>true</c> for active; <c>false</c> for inactive.
        /// </param>
        public override void SetActive(bool value)
        {
            if (line != null) NGUIDialogueUIControls.SetControlActive(line.gameObject, value);
            if (portraitImage != null) NGUIDialogueUIControls.SetControlActive(portraitImage.gameObject, value);
            if (portraitName != null) NGUIDialogueUIControls.SetControlActive(portraitName.gameObject, value);
            if (continueButton != null) NGUIDialogueUIControls.SetControlActive(continueButton.gameObject, value);
            if (panel != null) NGUIDialogueUIControls.SetControlActive(panel.gameObject, value);
        }

        public override void HideContinueButton()
        {
            if (continueButton != null) NGUIDialogueUIControls.SetControlActive(continueButton.gameObject, false);
        }

        /// <summary>
        /// Sets the subtitle.
        /// </summary>
        /// <param name='subtitle'>
        /// Subtitle.
        /// </param>
        public override void SetSubtitle(Subtitle subtitle)
        {
            if ((subtitle != null) && !string.IsNullOrEmpty(subtitle.formattedText.text))
            {
                if (portraitImage != null) portraitImage.mainTexture = subtitle.GetSpeakerPortrait();
                if (portraitName != null) portraitName.text = subtitle.speakerInfo.Name;
                if (line != null) SetFormattedText(line, subtitle.formattedText);
                Show();
                if (line != null)
                {
                    var typewriterEffect = line.GetComponent<TypewriterEffect>();
                    if (typewriterEffect != null)
                    {
                        typewriterEffect.ResetToBeginning();
                        DialogueManager.Instance.StartCoroutine(HideForOneFrame(line));
                    }
                }
            }
            else
            {
                if ((line != null) && (subtitle != null)) SetFormattedText(line, subtitle.formattedText);
                Hide();
            }
        }

        private IEnumerator HideForOneFrame(UILabel label)
        {
            Color originalColor = label.color;
            label.alpha = 0;
            yield return null;
            label.color = originalColor;
        }

        /// <summary>
        /// Clears the subtitle.
        /// </summary>
        public override void ClearSubtitle()
        {
            SetFormattedText(line, null);
        }

        /// <summary>
        /// Sets a label with formatted text.
        /// </summary>
        /// <param name='label'>
        /// Label to set.
        /// </param>
        /// <param name='formattedText'>
        /// Formatted text.
        /// </param>
        private void SetFormattedText(UILabel label, FormattedText formattedText)
        {
            if (label != null)
            {
                if (formattedText != null)
                {
                    label.text = formattedText.text;
                    if (Vector4.Magnitude(originalColor) == 0)
                    {
                        originalColor = label.color;
                    }
                    label.color = (formattedText.emphases.Length > 0) ? formattedText.emphases[0].color : originalColor;
                }
                else
                {
                    label.text = string.Empty;
                }
            }
        }

        /// <summary>
        /// Sets the portrait texture to use in the subtitle if the named actor is the speaker.
        /// This is used to immediately update the GUI control if the SetPortrait() sequencer 
        /// command changes the portrait texture.
        /// </summary>
        /// <param name="actorName">Actor name in database.</param>
        /// <param name="portraitTexture">Portrait texture.</param>
        public override void SetActorPortraitTexture(string actorName, Texture2D portraitTexture)
        {
            if ((currentSubtitle != null) && string.Equals(currentSubtitle.speakerInfo.nameInDatabase, actorName))
            {
                if (portraitImage != null) portraitImage.mainTexture = AbstractDialogueUI.GetValidPortraitTexture(actorName, portraitTexture);
            }
        }

    }

}
