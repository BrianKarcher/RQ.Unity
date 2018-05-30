using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.NGUI {

	/// <summary>
	/// This component hooks up the elements of an NGUI quest group template.
	/// Add it to your quest group template and assign the properties.
	/// </summary>
	[AddComponentMenu("Dialogue System/Third Party/NGUI/NGUI Quest Group Template")]
	public class NGUIQuestGroupTemplate : MonoBehaviour	{

		public UILabel heading;

		public bool ArePropertiesAssigned {
			get {
				return (heading != null);
			}
		}

	}

}
