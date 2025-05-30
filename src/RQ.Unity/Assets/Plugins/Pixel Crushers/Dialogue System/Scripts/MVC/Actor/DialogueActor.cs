// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// This component allows you to override the actor name used in conversations,
    /// which is normally set to the name of the GameObject. If the override name
    /// contains a [lua] or [var] tag, it parses the value.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class DialogueActor : MonoBehaviour
    {

        /// <summary>
        /// Overrides the actor name used in conversations.
        /// </summary>
        [Tooltip("Use this actor name in conversations.")]
        [ActorPopup(true)]
        [UnityEngine.Serialization.FormerlySerializedAs("overrideName")]
        public string actor;

        /// <summary>
        /// The internal name to use in the dialogue database when saving persistent data.
        /// If blank, uses the override name.
        /// </summary>
        [Tooltip("Name used when saving persistent data. If blank, use actor name.")]
        [UnityEngine.Serialization.FormerlySerializedAs("internalName")]        
        public string persistentDataName;

        [Tooltip("Optional portrait. If unassigned, will use portrait of actor in database.")]
        public Texture2D portrait;

        [Serializable]
        public class BarkUISettings
        {
            [Tooltip("If a prefab, Dialogue Actor will instantiate it at runtime.")]
            public AbstractBarkUI barkUI;

            [Tooltip("If instantiating bark UI prefab, offset it this far from Dialogue Actor's origin.")]
            public Vector3 barkUIOffset = new Vector3(0, 2, 0);
        }

        public BarkUISettings barkUISettings = new BarkUISettings();

        [Serializable]
        public class StandardDialogueUISettings
        {
            [Tooltip("If using Standard Dialogue UI, subtitle panel to use for this actor.")]
            public SubtitlePanelNumber subtitlePanelNumber = SubtitlePanelNumber.Default;

            [Tooltip("The panel to use if Subtitle Panel Number is set to Custom.")]
            public StandardUISubtitlePanel customSubtitlePanel = null;

            [Tooltip("If using Standard Dialogue UI, menu panel to use for this actor.")]
            public MenuPanelNumber menuPanelNumber = MenuPanelNumber.Default;

            [Tooltip("The panel to use if Menu Panel Number is set to Custom.")]
            public StandardUIMenuPanel customMenuPanel = null;

            [Tooltip("If assigned, animator controller that runs this actor's animated portrait. It should animate an Image component, not a SpriteRenderer.")]
            public RuntimeAnimatorController portraitAnimatorController;

            [Tooltip("Specify subtitle color for this actor.")]
            public bool setSubtitleColor = false;

            [Tooltip("Prepend actor name and apply color only to name.")]
            public bool applyColorToPrependedName = false;

            [Tooltip("If prepending actor name, separate from Dialogue Text with this string.")]
            public string prependActorNameSeparator = ": ";

            [Tooltip("Color to use for this actor's subtitles.")]
            public Color subtitleColor = Color.white;
        }

        public StandardDialogueUISettings standardDialogueUISettings = new StandardDialogueUISettings();

        private void Start()
        {
            SetupBarkUI();
            SetupDialoguePanels();
        }

        private void SetupBarkUI()
        {
            if (barkUISettings.barkUI != null && Tools.IsPrefab(barkUISettings.barkUI.gameObject))
            {
                // Instantiate bark UI from prefab:
                var go = Instantiate(barkUISettings.barkUI.gameObject) as GameObject;
                go.transform.SetParent(transform);
                go.transform.localPosition = barkUISettings.barkUIOffset;
                go.transform.localRotation = Quaternion.identity;
                barkUISettings.barkUI = go.GetComponent<AbstractBarkUI>();
            }
        }

        private void SetupDialoguePanels()
        {
            if (standardDialogueUISettings.subtitlePanelNumber == SubtitlePanelNumber.Custom &&
                standardDialogueUISettings.customSubtitlePanel != null &&
                Tools.IsPrefab(standardDialogueUISettings.customSubtitlePanel.gameObject))
            {
                // Instantiate subtitle panel from prefab:
                var go = Instantiate(standardDialogueUISettings.customSubtitlePanel.gameObject, transform.position, transform.rotation) as GameObject;
                go.transform.SetParent(transform);
                standardDialogueUISettings.customSubtitlePanel = go.GetComponent<StandardUISubtitlePanel>();
            }
            if (standardDialogueUISettings.menuPanelNumber == MenuPanelNumber.Custom &&
                standardDialogueUISettings.customMenuPanel != null &&
                Tools.IsPrefab(standardDialogueUISettings.customMenuPanel.gameObject))
            {
                // Instantiate menu panel from prefab:
                var go = Instantiate(standardDialogueUISettings.customMenuPanel.gameObject, transform.position, transform.rotation) as GameObject;
                go.transform.SetParent(transform);
                standardDialogueUISettings.customMenuPanel = go.GetComponent<StandardUIMenuPanel>();
            }
        }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(actor)) return;
            CharacterInfo.RegisterActorTransform(actor, transform);
        }

        private void OnDisable()
        {
            if (string.IsNullOrEmpty(actor)) return;
            CharacterInfo.UnregisterActorTransform(actor, transform);
        }

        /// <summary>
        /// Deprecated alias for GetActorName.
        /// </summary>
        public string GetName()
        {
            return GetActorName();
        }

        /// <summary>
        /// Gets the name to use for this DialogueActor, including parsing if it contains a [lua]
        /// or [var] tag.
        /// </summary>
        /// <returns>The name to use, or <c>null</c> if not set.</returns>
        public string GetActorName()
        {
            var actorName = string.IsNullOrEmpty(actor) ? name : actor;
            var result = CharacterInfo.GetLocalizedDisplayNameInDatabase(DialogueLua.GetActorField(actorName, "Name").asString);
            if (!string.IsNullOrEmpty(result)) actorName = result;
            if (actorName.Contains("[lua") || actor.Contains("[var"))
            {
                return FormattedText.Parse(actorName, DialogueManager.masterDatabase.emphasisSettings).text;
            }
            else
            {
                return actorName;
            }
        }

        /// <summary>
        /// Gets the name to use when saving persistent data.
        /// </summary>
        public string GetPersistentDataName()
        {
            return string.IsNullOrEmpty(persistentDataName) ? GetActorName() : persistentDataName;
        }

        /// <summary>
        /// Gets the panel number to use if using a Standard Dialogue UI.
        /// </summary>
        public SubtitlePanelNumber GetSubtitlePanelNumber()
        {
            return standardDialogueUISettings.subtitlePanelNumber;
        }

        /// <summary>
        /// Changes a dialogue actor's subtitle panel number. If a conversation is active, updates
        /// the dialogue UI.
        /// </summary>
        public void SetSubtitlePanelNumber(SubtitlePanelNumber newSubtitlePanelNumber)
        {
            standardDialogueUISettings.subtitlePanelNumber = newSubtitlePanelNumber;
            if (DialogueManager.isConversationActive && DialogueManager.dialogueUI is StandardDialogueUI)
            {
                (DialogueManager.dialogueUI as StandardDialogueUI).SetActorSubtitlePanelNumber(this, newSubtitlePanelNumber);
            }
        }

        /// <summary>
        /// Applies any color settings specified in the actor's standardDialogueUISettings.
        /// </summary>
        /// <param name="subtitle">The subtitle containing the source text.</param>
        /// <returns>The subtitle text adjusted for the actor's color settings.</returns>
        public string AdjustSubtitleColor(Subtitle subtitle)
        {
            var text = subtitle.formattedText.text;
            return !standardDialogueUISettings.setSubtitleColor ? text
                : (standardDialogueUISettings.applyColorToPrependedName ?
                    UITools.WrapTextInColor(subtitle.speakerInfo.Name + standardDialogueUISettings.prependActorNameSeparator, standardDialogueUISettings.subtitleColor) + text
                    : UITools.WrapTextInColor(text, standardDialogueUISettings.subtitleColor));
        }

        /// <summary>
        /// Searches a GameObject, including its parents and children, for a DialogueActor component.
        /// </summary>
        /// <param name="t">The GameObject to search.</param>
        /// <returns>The DialogueActor component, or null if not found.</returns>
        public static DialogueActor GetDialogueActorComponent(Transform t)
        {
            if (t == null) return null;
            return t.GetComponent<DialogueActor>() ?? t.GetComponentInChildren<DialogueActor>() ?? t.GetComponentInParent<DialogueActor>();
        }

        /// <summary>
        /// Gets the name of the actor, either from the GameObject or its DialogueActor
        /// if present.
        /// </summary>
        /// <returns>The actor name.</returns>
        /// <param name="t">The actor's transform.</param>
        public static string GetActorName(Transform t)
        {
            if (t == null) return string.Empty;
            var dialogueActor = GetDialogueActorComponent(t);
            return (dialogueActor == null) ? CharacterInfo.GetLocalizedDisplayNameInDatabase(t.name) : dialogueActor.GetName();
        }

        /// <summary>
        /// Gets the persistent data name of the actor, from the DialogueActor's persistentDataName
        /// if set, otherwise the actor name, or the GameObject name if the GameObject doesn't have a
        /// DialogueActor component.
        /// </summary>
        /// <param name="t">The actor's transform.</param>
        /// <returns></returns>
        public static string GetPersistentDataName(Transform t)
        {
            if (t == null) return string.Empty;
            var dialogueActor = GetDialogueActorComponent(t);
            if (dialogueActor != null)
            {
                if (!string.IsNullOrEmpty(dialogueActor.persistentDataName)) return dialogueActor.persistentDataName;
                if (!string.IsNullOrEmpty(dialogueActor.actor)) return dialogueActor.actor;
            }
            return t.name;
        }

        /// <summary>
        /// Gets the panel number to use if using a Standard Dialogue UI.
        /// </summary>
        /// <param name="t">The actor's transform.</param>
        public static SubtitlePanelNumber GetSubtitlePanelNumber(Transform t)
        {
            var dialogueActor = GetDialogueActorComponent(t);
            return (dialogueActor != null) ? dialogueActor.GetSubtitlePanelNumber() : SubtitlePanelNumber.Default;
        }

        public static IBarkUI GetBarkUI(Transform t)
        {
            if (t == null) return null;
            var dialogueActor = GetDialogueActorComponent(t);
            return (dialogueActor != null) ? (dialogueActor.barkUISettings.barkUI as IBarkUI) : t.GetComponentInChildren(typeof(IBarkUI)) as IBarkUI;
        }

    }

}
