using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.NGUI {

	/// <summary>
	/// When you attach this script to the Dialogue Manager object (or a child),
	/// it will display tracked quests using NGUI. It updates when the player
	/// toggles tracking in the quest log window and at the end of conversations. If you 
	/// change the state of a quest elsewhere, you must manually call UpdateTracker().
	/// </summary>
	[AddComponentMenu("Dialogue System/Third Party/NGUI/NGUI Quest Tracker (on Dialogue Manager)")]
	public class NGUIQuestTracker : MonoBehaviour {

		/// <summary>
		/// The UI control that will hold quest track info (instantiated copies of the 
		/// quest track template). This is typically a Vertical Layout Group.
		/// </summary>
		public Transform container;

		/// <summary>
		/// The quest track template.
		/// </summary>
		public NGUIQuestTrackTemplate questTrackTemplate;

		public enum QuestDescriptionSource { Title, Description }

		public QuestDescriptionSource questDescriptionSource = QuestDescriptionSource.Title;

		private List<GameObject> instantiatedItems = new List<GameObject>();

		/// <summary>
		/// Wait 0.5s to update the tracker in case other start
		/// methods change the state of quests.
		/// </summary>
		public void Start() {
			if (container == null) Debug.LogWarning(string.Format("{0}: {1} Container is unassigned", new object[] { DialogueDebug.Prefix, name }), this);
			if (questTrackTemplate == null) {
				Debug.LogWarning(string.Format("{0}: {1} Quest Track Template is unassigned", new object[] { DialogueDebug.Prefix, name }), this);
			} else {
				NGUITools.SetActive(questTrackTemplate.gameObject, false);
			}
			Invoke("UpdateTracker", 0.5f);
		}

		/// <summary>
		/// The quest log window sends this message when the player toggles tracking.
		/// </summary>
		/// <param name="quest">Quest.</param>
		public void OnQuestTrackingEnabled(string quest) {
			UpdateTracker();
		}
		
		/// <summary>
		/// The quest log window sends this message when the player toggles tracking.
		/// </summary>
		/// <param name="quest">Quest.</param>
		public void OnQuestTrackingDisabled(string quest) {
			UpdateTracker();
		}

		/// <summary>
		/// Quests are often completed in conversations. This handles changes in quest states
		/// after conversations.
		/// </summary>
		/// <param name="actor">Actor.</param>
		public void OnConversationEnd(Transform actor) {
			UpdateTracker();
		}
		
		public void UpdateTracker() {
			DestroyInstantiatedItems();
			foreach (string quest in QuestLog.GetAllQuests()) {
				if (QuestLog.IsQuestActive(quest) && QuestLog.IsQuestTrackingEnabled(quest)) {
					InstantiateQuestTrack(quest);
				}
			}
		}

		public void DestroyInstantiatedItems() {
			foreach (var go in instantiatedItems) {
				Destroy(go);
			}
			instantiatedItems.Clear();
		}

		private void InstantiateQuestTrack(string quest) {
			if (container == null || questTrackTemplate == null) return;
			var go = Instantiate(questTrackTemplate.gameObject) as GameObject;
			if (go == null) {
				Debug.LogError(string.Format("{0}: {1} couldn't instantiate quest track template", new object[] { DialogueDebug.Prefix, name }), this);
				return;
			}
			instantiatedItems.Add(go);
			var heading = GetQuestDescription(quest);
			go.name = heading;
			go.transform.parent = container.transform;
			NGUITools.SetActive(go, true);
			var questTrack = go.GetComponent<NGUIQuestTrackTemplate>();
			if (questTrack.description != null) questTrack.description.text = heading;
			if (questTrack.entryDescription != null) {
				var entryDescription = GetQuestEntryDescription(quest);
				questTrack.entryDescription.text = entryDescription;
				NGUITools.SetActive(questTrack.entryDescription.gameObject, !string.IsNullOrEmpty(entryDescription));
			}
			go.transform.localScale = questTrackTemplate.transform.localScale;
			var uiTable = container.GetComponent<UITable>();
			if (uiTable != null) uiTable.Reposition();
		}

		private string GetQuestDescription(string quest) {
			var sourceText = (questDescriptionSource == QuestDescriptionSource.Title) 
				? QuestLog.GetQuestTitle(quest)
				: QuestLog.GetQuestDescription(quest);
			return FormattedText.Parse(sourceText, DialogueManager.MasterDatabase.emphasisSettings).text;
		}

		private string GetQuestEntryDescription(string quest) {
			StringBuilder sb = new StringBuilder();
			int entryCount = QuestLog.GetQuestEntryCount(quest);
			for (int i = 1; i <= entryCount; i++) {
				sb.Append(FormattedText.Parse(QuestLog.GetQuestEntry(quest, i), DialogueManager.MasterDatabase.emphasisSettings).text);
				if (i < entryCount) sb.Append("\n");
			}
			return sb.ToString();
		}

	}

}
