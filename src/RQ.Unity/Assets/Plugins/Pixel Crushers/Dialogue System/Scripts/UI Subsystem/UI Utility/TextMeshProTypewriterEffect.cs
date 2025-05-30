﻿#if TMP_PRESENT
// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// This is a typewriter effect for TextMesh Pro.
    /// 
    /// Note: Handles RPGMaker codes, but not two codes next to each other.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    [DisallowMultipleComponent]
    public class TextMeshProTypewriterEffect : AbstractTypewriterEffect
    {

        [System.Serializable]
        public class AutoScrollSettings
        {
            [Tooltip("Automatically scroll to bottom of scroll rect. Useful for long text. Works best with left justification.")]
            public bool autoScrollEnabled = false;
            public UnityEngine.UI.ScrollRect scrollRect = null;
            public UnityUIScrollbarEnabler scrollbarEnabler = null;
        }

        /// <summary>
        /// Optional auto-scroll settings.
        /// </summary>
        public AutoScrollSettings autoScrollSettings = new AutoScrollSettings();

        public UnityEvent onBegin = new UnityEvent();
        public UnityEvent onCharacter = new UnityEvent();
        public UnityEvent onEnd = new UnityEvent();

        /// <summary>
        /// Indicates whether the effect is playing.
        /// </summary>
        /// <value><c>true</c> if this instance is playing; otherwise, <c>false</c>.</value>
        public override bool isPlaying { get { return typewriterCoroutine != null; } }

        /// @cond FOR_V1_COMPATIBILITY
        public bool IsPlaying { get { return isPlaying; } }
        /// @endcond
        
        private const string RPGMakerCodeQuarterPause = @"\,";
        private const string RPGMakerCodeFullPause = @"\.";
        private const string RPGMakerCodeSkipToEnd = @"\^";
        private const string RPGMakerCodeInstantOpen = @"\>";
        private const string RPGMakerCodeInstantClose = @"\<";

        private enum RPGMakerTokenType
        {
            None,
            QuarterPause,
            FullPause,
            SkipToEnd,
            InstantOpen,
            InstantClose
        }

        private Dictionary<int, List<RPGMakerTokenType>> rpgMakerTokens = new Dictionary<int, List<RPGMakerTokenType>>();

        private TMPro.TMP_Text m_textComponent = null;
        private TMPro.TMP_Text textComponent
        {
            get
            {
                if (m_textComponent == null) m_textComponent = GetComponent<TMPro.TMP_Text>();
                return m_textComponent;
            }
        }

        private AudioSource runtimeAudioSource
        {
            get
            {
                if (audioSource == null) audioSource = GetComponent<AudioSource>();
                if (audioSource == null && (audioClip != null))
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.playOnAwake = false;
                    audioSource.panStereo = 0;
                }
                return audioSource;
            }
        }

        private bool started = false;
        private bool paused = false;
        private int charactersTyped = 0;
        private Coroutine typewriterCoroutine = null;
        private MonoBehaviour coroutineController = null;

        public override void Awake()
        {
            
            if (removeDuplicateTypewriterEffects) RemoveIfDuplicate();
        }

        private void RemoveIfDuplicate()
        {
            var effects = GetComponents<TextMeshProTypewriterEffect>();
            if (effects.Length > 1)
            {
                var keep = effects[0];
                for (int i = 1; i < effects.Length; i++)
                {
                    if (effects[i].GetInstanceID() < keep.GetInstanceID())
                    {
                        keep = effects[i];
                    }
                }
                for (int i = 0; i < effects.Length; i++)
                {
                    if (effects[i] != keep)
                    {
                        Destroy(effects[i]);
                    }
                }
            }
        }

        public override void Start()
        {
            if (!IsPlaying && playOnEnable)
            {
                StopTypewriterCoroutine();
                StartTypewriterCoroutine(0);
            }
            started = true;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (!IsPlaying && playOnEnable && started)
            {
                StopTypewriterCoroutine();
                StartTypewriterCoroutine(0);
            }
        }

        public override void OnDisable()
        {
            base.OnEnable();
            Stop();
        }

        /// <summary>
        /// Pauses the effect.
        /// </summary>
        public void Pause()
        {
            paused = true;
        }

        /// <summary>
        /// Unpauses the effect. The text will resume at the point where it
        /// was paused; it won't try to catch up to make up for the pause.
        /// </summary>
        public void Unpause()
        {
            paused = false;
        }

        public void Rewind()
        {
            charactersTyped = 0;
        }

        /// <summary>
        /// Starts typing, optionally from a starting index. Characters before the 
        /// starting index will appear immediately.
        /// </summary>
        /// <param name="text">Text to type.</param>
        /// <param name="fromIndex">Character index to start typing from.</param>
        public override void StartTyping(string text, int fromIndex = 0)
        {
            StopTypewriterCoroutine();
            textComponent.text = text;
            StartTypewriterCoroutine(fromIndex);
        }

        public override void StopTyping()
        {
            Stop();
        }

        /// <summary>
        /// Play typewriter on text immediately.
        /// </summary>
        /// <param name="text"></param>
        public void PlayText(string text, int fromIndex = 0)
        {
            StopTypewriterCoroutine();
            textComponent.text = text;
            StartTypewriterCoroutine(fromIndex);
        }

        private void StartTypewriterCoroutine(int fromIndex)
        {
            if (coroutineController == null || !coroutineController.gameObject.activeInHierarchy)
            {
                // This MonoBehaviour might not be enabled yet, so use one that's guaranteed to be enabled:
                MonoBehaviour controller = GetComponentInParent<AbstractDialogueUI>();
                if (controller == null) controller = DialogueManager.instance;
                coroutineController = controller;
                if (coroutineController == null) coroutineController = this;
            }
            typewriterCoroutine = coroutineController.StartCoroutine(Play(fromIndex));
        }

        /// <summary>
        /// Plays the typewriter effect.
        /// </summary>
        public IEnumerator Play(int fromIndex)
        {
            if ((textComponent != null) && (charactersPerSecond > 0))
            {
                if (waitOneFrameBeforeStarting) yield return null;
                ProcessRPGMakerCodes();
                if (runtimeAudioSource != null) runtimeAudioSource.clip = audioClip;
                onBegin.Invoke();
                paused = false;
                float delay = 1 / charactersPerSecond;
                float lastTime = DialogueTime.time;
                float elapsed = fromIndex / charactersPerSecond;
                textComponent.maxVisibleCharacters = 0;
                textComponent.ForceMeshUpdate();
                yield return null;
                textComponent.maxVisibleCharacters = 0;
                textComponent.ForceMeshUpdate();
                TMPro.TMP_TextInfo textInfo = textComponent.textInfo;
                int totalVisibleCharacters = textInfo.characterCount; // Get # of Visible Character in text object
                charactersTyped = fromIndex;
                int skippedCharacters = 0;
                while (charactersTyped < totalVisibleCharacters)
                {
                    if (!paused)
                    {
                        var deltaTime = DialogueTime.time - lastTime;
                        elapsed += deltaTime;
                        var goal = (elapsed * charactersPerSecond) - skippedCharacters;
                        while (charactersTyped < goal)
                        {
                            if (rpgMakerTokens.ContainsKey(charactersTyped))
                            {
                                var tokens = rpgMakerTokens[charactersTyped];
                                for (int i = 0; i < tokens.Count; i++)
                                {
                                    var token = tokens[i];
                                    switch (token)
                                    {
                                        case RPGMakerTokenType.QuarterPause:
                                            yield return new WaitForSeconds(quarterPauseDuration);
                                            break;
                                        case RPGMakerTokenType.FullPause:
                                            yield return new WaitForSeconds(fullPauseDuration);
                                            break;
                                        case RPGMakerTokenType.SkipToEnd:
                                            charactersTyped = totalVisibleCharacters - 1;
                                            break;
                                        case RPGMakerTokenType.InstantOpen:
                                            var close = false;
                                            while (!close && charactersTyped < totalVisibleCharacters)
                                            {
                                                charactersTyped++;
                                                skippedCharacters++;
                                                if (rpgMakerTokens.ContainsKey(charactersTyped) && rpgMakerTokens[charactersTyped].Contains(RPGMakerTokenType.InstantClose))
                                                {
                                                    close = true;
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            if (charactersTyped < totalVisibleCharacters && !IsSilentCharacter(textComponent.text[charactersTyped])) PlayCharacterAudio();
                            onCharacter.Invoke();
                            charactersTyped++;
                            textComponent.maxVisibleCharacters = charactersTyped;
                        }
                    }
                    textComponent.maxVisibleCharacters = charactersTyped;
                    HandleAutoScroll();
                    //---Uncomment the line below to debug: 
                    //Debug.Log(textComponent.text.Substring(0, charactersTyped).Replace("<", "[").Replace(">", "]") + " (typed=" + charactersTyped + ")");
                    lastTime = DialogueTime.time;
                    var delayTime = DialogueTime.time + delay;
                    int delaySafeguard = 0;
                    while (DialogueTime.time < delayTime && delaySafeguard < 999)
                    {
                        delaySafeguard++;
                        yield return null;
                    }
                }
            }
            Stop();
        }

        private void ProcessRPGMakerCodes()
        {
            rpgMakerTokens.Clear();
            var source = textComponent.text;
            var result = string.Empty;
            if (!source.Contains("\\")) return;
            int safeguard = 0;
            while (!string.IsNullOrEmpty(source) && safeguard < 9999)
            {
                safeguard++;
                RPGMakerTokenType token;
                if (PeelRPGMakerTokenFromFront(ref source, out token))
                {
                    int i = result.Length;
                    if (!rpgMakerTokens.ContainsKey(i))
                    {
                        rpgMakerTokens.Add(i, new List<RPGMakerTokenType>());
                    }
                    rpgMakerTokens[i].Add(token);
                }
                else
                {
                    result += source[0];
                    source = source.Remove(0, 1);
                }
            }
            textComponent.text = result;
        }

        private bool PeelRPGMakerTokenFromFront(ref string source, out RPGMakerTokenType token)
        {
            token = RPGMakerTokenType.None;
            if (string.IsNullOrEmpty(source) || source.Length < 2 || source[0] != '\\') return false;
            var s = source.Substring(0, 2);
            if (string.Equals(s, RPGMakerCodeQuarterPause))
            {
                token = RPGMakerTokenType.QuarterPause;
            }
            else if (string.Equals(s, RPGMakerCodeFullPause))
            {
                token = RPGMakerTokenType.FullPause;
            }
            else if (string.Equals(s, RPGMakerCodeSkipToEnd))
            {
                token = RPGMakerTokenType.SkipToEnd;
            }
            else if (string.Equals(s, RPGMakerCodeInstantOpen))
            {
                token = RPGMakerTokenType.InstantOpen;
            }
            else if (string.Equals(s, RPGMakerCodeInstantClose))
            {
                token = RPGMakerTokenType.InstantClose;
            }
            else
            {
                return false;
            }
            source = source.Remove(0, 2);
            return true;
        }

        private bool IsSilentCharacter(char c)
        {
            if (string.IsNullOrEmpty(silentCharacters)) return false;
            return silentCharacters.Contains(c.ToString());
        }

        private void PlayCharacterAudio()
        {
            if (audioClip == null || audioSource == null) return;
            AudioClip randomClip = null;
            if (alternateAudioClips != null && alternateAudioClips.Length > 0)
            {
                var randomIndex = UnityEngine.Random.Range(0, alternateAudioClips.Length + 1);
                randomClip = (randomIndex < alternateAudioClips.Length) ? alternateAudioClips[randomIndex] : audioClip;
            }
            if (interruptAudioClip)
            {
                if (audioSource.isPlaying) audioSource.Stop();
                if (randomClip != null) audioSource.clip = randomClip;
                audioSource.Play();
            }
            else
            {
                if (!audioSource.isPlaying)
                {
                    if (randomClip != null) audioSource.clip = randomClip;
                    audioSource.Play();
                }
            }
        }

        private void StopTypewriterCoroutine()
        {
            if (typewriterCoroutine == null) return;
            if (coroutineController == null)
            {
                StopCoroutine(typewriterCoroutine);
            }
            else
            {
                coroutineController.StopCoroutine(typewriterCoroutine);
            }
            typewriterCoroutine = null;
            coroutineController = null;
        }

        /// <summary>
        /// Stops the effect.
        /// </summary>
        public override void Stop()
        {
            if (isPlaying) onEnd.Invoke();
            Sequencer.Message(SequencerMessages.Typed);
            StopTypewriterCoroutine();
            if (textComponent != null) textComponent.maxVisibleCharacters = textComponent.textInfo.characterCount;
            HandleAutoScroll();
        }

        private void HandleAutoScroll()
        {
            if (!autoScrollSettings.autoScrollEnabled) return;
            if (autoScrollSettings.scrollRect != null)
            {
                autoScrollSettings.scrollRect.normalizedPosition = new Vector2(0, 0);
            }
            if (autoScrollSettings.scrollbarEnabler != null)
            {
                autoScrollSettings.scrollbarEnabler.CheckScrollbar();
            }
        }

    }

}
#endif
