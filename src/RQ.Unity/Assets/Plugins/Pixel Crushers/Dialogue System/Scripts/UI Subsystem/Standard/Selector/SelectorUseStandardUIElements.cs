﻿// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// Tells the Selector/ProximitySelector to use StandardUISelectorElements
    /// to show the current selection.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class SelectorUseStandardUIElements : MonoBehaviour
    {

        private Selector selector = null;
        private ProximitySelector proximitySelector = null;
        private string defaultUseMessage = string.Empty;
        private Usable usable = null;
        private bool lastInRange = false;
        private AbstractUsableUI usableUI = null;
        private bool started = false;

        protected float CurrentDistance
        {
            get
            {
                return (selector != null) ? selector.CurrentDistance : 0;
            }
        }

        private StandardUISelectorElements elements { get { return StandardUISelectorElements.instance; } }

        public void Start()
        {
            if (elements == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: SelectorUseStandardUIElements can't find a StandardUISelectorElements component in the scene.", this);
                enabled = false;
            }
            else
            {
                started = true;
                ConnectDelegates();
                DeactivateControls();
            }
        }

        public void OnEnable()
        {
            if (started) ConnectDelegates();
        }

        public void OnDisable()
        {
            DisconnectDelegates();
        }

        private void ConnectDelegates()
        {
            DisconnectDelegates(); // Make sure we're not connecting twice.
            selector = GetComponent<Selector>();
            if (selector != null)
            {
                selector.useDefaultGUI = false;
                selector.SelectedUsableObject += OnSelectedUsable;
                selector.DeselectedUsableObject += OnDeselectedUsable;
                defaultUseMessage = selector.defaultUseMessage;
            }
            proximitySelector = GetComponent<ProximitySelector>();
            if (proximitySelector != null)
            {
                proximitySelector.useDefaultGUI = false;
                proximitySelector.SelectedUsableObject += OnSelectedUsable;
                proximitySelector.DeselectedUsableObject += OnDeselectedUsable;
                if (string.IsNullOrEmpty(defaultUseMessage)) defaultUseMessage = proximitySelector.defaultUseMessage;
            }
        }

        private void DisconnectDelegates()
        {
            selector = GetComponent<Selector>();
            if (selector != null)
            {
                selector.useDefaultGUI = true;
                selector.SelectedUsableObject -= OnSelectedUsable;
                selector.DeselectedUsableObject -= OnDeselectedUsable;
            }
            proximitySelector = GetComponent<ProximitySelector>();
            if (proximitySelector != null)
            {
                proximitySelector.useDefaultGUI = true;
                proximitySelector.SelectedUsableObject -= OnSelectedUsable;
                proximitySelector.DeselectedUsableObject -= OnDeselectedUsable;
            }
            HideControls();
        }

        private void OnSelectedUsable(Usable usable)
        {
            this.usable = usable;
            usableUI = (usable != null) ? usable.GetComponentInChildren<AbstractUsableUI>() : null;
            if (usableUI != null)
            {
                usableUI.Show(GetUseMessage());
            }
            else
            {
                ShowControls();
            }
            lastInRange = !IsUsableInRange();
            UpdateDisplay(!lastInRange);
        }

        private void OnDeselectedUsable(Usable usable)
        {
            if (usableUI != null)
            {
                usableUI.Hide();
                usableUI = null;
            }
            else
            {
                HideControls();
            }
            this.usable = null;
        }

        private string GetUseMessage()
        {
            return string.IsNullOrEmpty(usable.overrideUseMessage) ? defaultUseMessage : usable.overrideUseMessage;
        }

        private void ShowControls()
        {
            if (usable == null || elements == null) return;
            Tools.SetGameObjectActive(elements.mainGraphic, true);
            elements.nameText.SetActive(true);
            elements.useMessageText.SetActive(true);
            elements.nameText.text = usable.GetName();
            elements.useMessageText.text = GetUseMessage();
            Tools.SetGameObjectActive(elements.reticleInRange, IsUsableInRange());
            Tools.SetGameObjectActive(elements.reticleOutOfRange, !IsUsableInRange());
            if (CanTriggerAnimations() && !string.IsNullOrEmpty(elements.animationTransitions.showTrigger))
            {
                elements.animator.SetTrigger(elements.animationTransitions.showTrigger);
            }
        }

        private void HideControls()
        {
            if (CanTriggerAnimations() && elements != null && !string.IsNullOrEmpty(elements.animationTransitions.hideTrigger))
            {
                elements.animator.SetTrigger(elements.animationTransitions.hideTrigger);
            }
            else
            {
                DeactivateControls();
            }
        }

        private void DeactivateControls()
        {
            if (elements == null) return;
            elements.nameText.SetActive(false);
            elements.useMessageText.SetActive(false);
            Tools.SetGameObjectActive(elements.reticleInRange, false);
            Tools.SetGameObjectActive(elements.reticleOutOfRange, false);
            Tools.SetGameObjectActive(elements.mainGraphic, false);
        }

        private bool IsUsableInRange()
        {
            return (usable != null) && (CurrentDistance <= usable.maxUseDistance);
        }

        public void Update()
        {
            if (usable != null)
            {
                UpdateDisplay(IsUsableInRange());
            }
        }

        public void OnConversationStart(Transform actor)
        {
            HideControls();
        }

        public void OnConversationEnd(Transform actor)
        {
            ShowControls();
        }

        private void UpdateDisplay(bool inRange)
        {
            if ((usable != null) && (inRange != lastInRange))
            {
                lastInRange = inRange;
                if (usableUI != null)
                {
                    usableUI.UpdateDisplay(inRange);
                }
                else
                {
                    UpdateText(inRange);
                    UpdateReticle(inRange);
                }
            }
        }

        private void UpdateText(bool inRange)
        {
            if (elements == null) return;
            var color = inRange ? elements.inRangeColor : elements.outOfRangeColor;
            if (elements.nameText != null) elements.nameText.color = color;
            if (elements.useMessageText != null) elements.useMessageText.color = color;
        }

        private void UpdateReticle(bool inRange)
        {
            if (elements == null) return;
            Tools.SetGameObjectActive(elements.reticleInRange, inRange);
            Tools.SetGameObjectActive(elements.reticleOutOfRange, !inRange);
        }

        private bool CanTriggerAnimations()
        {
            return (elements != null) && (elements.animator != null) && (elements.animationTransitions != null);
        }

    }

}
