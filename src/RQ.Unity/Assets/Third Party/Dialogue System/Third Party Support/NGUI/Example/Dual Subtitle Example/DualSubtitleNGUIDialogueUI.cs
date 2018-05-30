using UnityEngine;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.NGUI;

namespace PixelCrushers.DialogueSystem.NGUI {

	/// <summary>
	/// This class is an example of overriding NGUIDialogueUI to modify
	/// its behavior. It allows dual subtitles.
	/// </summary>
	public class DualSubtitleNGUIDialogueUI : NGUIDialogueUI {

		public override void ShowSubtitle(Subtitle subtitle) {
			if (subtitle.speakerInfo.IsPlayer) {
				DirectSubtitle(subtitle, Dialogue.PCSubtitle);
			} else if (subtitle.speakerInfo.transform == DialogueManager.CurrentConversant) {
				DirectSubtitle(subtitle, Dialogue.NPCSubtitle);
			}
		}

		private void DirectSubtitle(Subtitle subtitle, AbstractUISubtitleControls subtitleControls) {
			subtitleControls.Hide();
			subtitleControls.ShowSubtitle(subtitle);
		}

		public override void HideSubtitle(Subtitle subtitle) {
			//--- Skip this: base.HideSubtitle (subtitle);
		}

		public override void ShowResponses(Subtitle subtitle, Response[] responses, float timeout) {
			Dialogue.PCSubtitle.Hide(); // Hide PC subtitle before showing PC response menu.
			base.ShowResponses(subtitle, responses, timeout);
		}

	}

}
