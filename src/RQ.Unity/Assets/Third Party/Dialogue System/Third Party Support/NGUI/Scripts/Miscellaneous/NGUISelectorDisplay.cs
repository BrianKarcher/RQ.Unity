using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.NGUI {

	/// <summary>
	/// Replaces the Selector/ProximitySelector's OnGUI method with a method
	/// that enables or disables an NGUI UILabel and optional main control widget.
	/// </summary>
	[AddComponentMenu("Dialogue System/Third Party/NGUI/Selector Display")]
	public class NGUISelectorDisplay : MonoBehaviour {
	
		/// <summary>
		/// The main control (optional). Assign this if you have created an entire 
		/// panel for the selector.
		/// </summary>
		[Tooltip("The main control (optional). Assign this if you have created an entire panel for the selector or if Follow Target is ticked.")]
		public UIWidget mainControl = null;
		
		/// <summary>
		/// The UILabel for the name of the current selection.
		/// </summary>
		[Tooltip("The UILabel for the name of the current selection.")]
		public UILabel nameLabel = null;

		/// <summary>
		/// The UILabel for the use message (e.g., "Press spacebar to use").
		/// </summary>
		[Tooltip("The UILabel for the use message (e.g., 'Press spacebar to use').")]
		public UILabel useMessageLabel = null;

		/// <summary>
		/// The widget to show if the selection is in range.
		/// </summary>
		[Tooltip("The widget to show if the selection is in range.")]
		public UIWidget reticleInRange = null;

		/// <summary>
		/// The widget to show if the selection is out of range.
		/// </summary>
		[Tooltip("The widget to show if the selection is out of range.")]
		public UIWidget reticleOutOfRange = null;

		/// <summary>
		/// Position the mainControl on the selection target.
		/// </summary>
		[Tooltip("Position the mainControl on top of the selection target.")]
		public bool followTarget = false;

		private string defaultUseMessage = string.Empty;

		private Selector selector = null;

		private ProximitySelector proximitySelector = null;

		private Usable usable = null;

		protected float CurrentDistance {
			get {
				return (selector != null) ? selector.CurrentDistance : 0;
			}
		}

		public void Start() {
			ConnectDelegates();
		}
		
		public void OnEnable() {
			ConnectDelegates();
		}

		public void OnDisable() {
			DisconnectDelegates();
		}

		private void ConnectDelegates() {
			DisconnectDelegates(); // Make sure we're not connecting twice.
			selector = GetComponent<Selector>();
			if (selector != null) {
				selector.useDefaultGUI = false;
				selector.SelectedUsableObject += OnSelectedUsable;
				selector.DeselectedUsableObject += OnDeselectedUsable;
				defaultUseMessage = selector.defaultUseMessage;
			}
			proximitySelector = GetComponent<ProximitySelector>();
			if (proximitySelector != null) {
				proximitySelector.useDefaultGUI = false;
				proximitySelector.SelectedUsableObject += OnSelectedUsable;
				proximitySelector.DeselectedUsableObject += OnDeselectedUsable;
				if (string.IsNullOrEmpty(defaultUseMessage)) defaultUseMessage = proximitySelector.defaultUseMessage;
			}
		}
		
		private void DisconnectDelegates() {
			selector = GetComponent<Selector>();
			if (selector != null) {
				selector.useDefaultGUI = true;
				selector.SelectedUsableObject -= OnSelectedUsable;
				selector.DeselectedUsableObject -= OnDeselectedUsable;
			}
			proximitySelector = GetComponent<ProximitySelector>();
			if (proximitySelector != null) {
				proximitySelector.useDefaultGUI = true;
				proximitySelector.SelectedUsableObject -= OnSelectedUsable;
				proximitySelector.DeselectedUsableObject -= OnDeselectedUsable;
			}
			HideControls();
		}
		
		private void OnSelectedUsable(Usable usable) {
			this.usable = usable;
			if (mainControl != null) {
				NGUITools.SetActive(mainControl.gameObject, true);
				if (followTarget) {
					var nguiFollowTarget = mainControl.GetComponent<NGUIFollowTarget>();
					if (nguiFollowTarget == null) nguiFollowTarget = mainControl.gameObject.AddComponent<NGUIFollowTarget>();
					nguiFollowTarget.target = usable.transform;
				}
			}
			if (nameLabel != null) {
				NGUITools.SetActive(nameLabel.gameObject, true);
				nameLabel.text = usable.GetName();
			}
			if (useMessageLabel != null) {
				NGUITools.SetActive(useMessageLabel.gameObject, true);
				useMessageLabel.text = string.IsNullOrEmpty(usable.overrideUseMessage) ? defaultUseMessage : usable.overrideUseMessage;
			}
			UpdateReticle();
		}
		
		private void OnDeselectedUsable(Usable usable) {
			HideControls();
			this.usable = null;
		}

		protected void HideControls() {
			if (nameLabel != null) NGUITools.SetActive(nameLabel.gameObject, false);
			if (reticleInRange != null) NGUITools.SetActive(reticleInRange.gameObject, false);
			if (reticleOutOfRange != null) NGUITools.SetActive(reticleOutOfRange.gameObject, false);
			if (useMessageLabel != null) NGUITools.SetActive(useMessageLabel.gameObject, false);
			if (mainControl != null) NGUITools.SetActive(mainControl.gameObject, false);
		}

		public void Update() {
			if (usable != null) UpdateReticle();
		}

		protected void UpdateReticle() {
			if (usable == null) return;
			bool inRange = (CurrentDistance <= usable.maxUseDistance);
			if (reticleInRange != null) NGUITools.SetActive(reticleInRange.gameObject, inRange);
			if (reticleOutOfRange != null) NGUITools.SetActive(reticleOutOfRange.gameObject, !inRange);
		}

	}

}