using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.NGUI {

	/// <summary>
	/// This component hooks up the elements of an NGUI quest track template,
	/// which is used by the NGUI Quest Tracker.
	/// Add it to your quest track template and assign the properties.
	/// </summary>
	[AddComponentMenu("Dialogue System/Third Party/NGUI/NGUI Quest Track Template")]
	public class NGUIQuestTrackTemplate : MonoBehaviour	{

		public UILabel description;

		public UILabel entryDescription;

		public bool ArePropertiesAssigned {
			get {
				return (description != null) && (entryDescription != null);
			}
		}

	}

}
