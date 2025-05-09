﻿// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Events;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// This is a typewriter effect for Unity UI. It handles bold, italic, quad, and
    /// color rich text tags and certain RPGMaker-style tags. It also works with any text alignment.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    [DisallowMultipleComponent]
    public class UnityUITypewriterEffect : AbstractTypewriterEffect
    {

        [System.Serializable]
        public class AutoScrollSettings
        {
            [Tooltip("Automatically scroll to bottom of scroll rect. Useful for long text. Works best with left justification. Make sure the text has a Content Size Fitter.")]
            public bool autoScrollEnabled = false;

            public UnityEngine.UI.ScrollRect scrollRect = null;

            [Tooltip("If assigned, the Scrollbar Enabler will be updated with each character to determine if it needs to show the scrollbar.")]
            public UIScrollbarEnabler scrollbarEnabler = null;

            [Tooltip("If assigned, this should be a copy of the Text component on this typewriter effect. The Sizer Text should have a Content Size Fitter, but the typewriter Text component should not. Make the Sizer Text a parent of the typewriter Text component.")]
            public UnityEngine.UI.Text sizerText = null;
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

        private const string RichTextBoldOpen = "<b>";
        private const string RichTextBoldClose = "</b>";
        private const string RichTextItalicOpen = "<i>";
        private const string RichTextItalicClose = "</i>";
        private const string RichTextColorOpenPrefix = "<color=";
        private const string RichTextColorClose = "</color>";
        private const string RichTextSizeOpenPrefix = "<size=";
        private const string RichTextSizeClose = "</size>";
        private const string QuadPrefix = "<quad ";

        private enum TokenType
        {
            Character,
            BoldOpen,
            BoldClose,
            ItalicOpen,
            ItalicClose,
            ColorOpen,
            ColorClose,
            SizeOpen,
            SizeClose,
            Quad,
            Pause,
            InstantOpen,
            InstantClose
        }

        private class Token
        {
            public TokenType tokenType;
            public char character;
            public string code;
            public float duration;

            public Token(TokenType tokenType, char character, string code, float duration)
            {
                this.tokenType = tokenType;
                this.character = character;
                this.code = code;
                this.duration = duration;
            }
        }

        private UnityEngine.UI.Text control;
        private bool started = false;
        private bool paused = false;
        private string original = null;
        private string frontSkippedText = string.Empty;
        private Coroutine typewriterCoroutine = null;
        private MonoBehaviour coroutineController = null;

        private StringBuilder current;
        private List<TokenType> openTokenTypes;
        private List<Token> tokens;

        private int MaxSafeguard = 16384;

        public override void Awake()
        {
            control = GetComponent<UnityEngine.UI.Text>();
            if (removeDuplicateTypewriterEffects) RemoveIfDuplicate();
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
            if (audioSource == null && (audioClip != null || (alternateAudioClips != null && alternateAudioClips.Length > 0)))
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
#if UNITY_4_6 || UNITY_4_7
                audioSource.pan = 0;
#else
                audioSource.panStereo = 0;
#endif
            }
        }

        private void RemoveIfDuplicate()
        {
            var effects = GetComponents<UnityUITypewriterEffect>();
            if (effects.Length > 1)
            {
                UnityUITypewriterEffect keep = effects[0];
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
            if (control != null) control.supportRichText = true;
            if (!IsPlaying && playOnEnable)
            {
                original = null;
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
                original = null;
                StopTypewriterCoroutine();
                StartTypewriterCoroutine(0);
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
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

        public override void StartTyping(string text, int fromIndex = 0)
        {
            StopTypewriterCoroutine();
            original = text;
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
            StartTyping(text, fromIndex);
        }

        private void StartTypewriterCoroutine(int fromIndex)
        {
            if (coroutineController == null || !coroutineController.gameObject.activeInHierarchy)
            {
                // This MonoBehaviour might not be enabled yet, so use one that's guaranteed to be enabled:
                MonoBehaviour controller = GetComponentInParent<UnityUIDialogueUI>();
                if (controller == null) controller = DialogueManager.instance;
                coroutineController = controller;
                if (coroutineController == null) coroutineController = this;
            }
            typewriterCoroutine = coroutineController.StartCoroutine(Play(fromIndex));
        }

        /// <summary>
        /// Plays the typewriter effect.
        /// </summary>
        public IEnumerator Play(int fromIndex = 0)
        {
            if ((control != null) && (charactersPerSecond > 0))
            {
                // Setup:
                InitAutoScroll();
                if (waitOneFrameBeforeStarting) yield return null;
                if (audioSource != null) audioSource.clip = audioClip;
                onBegin.Invoke();
                paused = false;
                float delay = 1 / charactersPerSecond;
                float lastTime = DialogueTime.time;
                float elapsed = 0;
                int charactersTyped = 0;
                if (original == null) original = control.text;
                frontSkippedText = string.Empty;
                if (fromIndex > 0)
                {
                    frontSkippedText = original.Substring(0, fromIndex);
                    original = original.Substring(fromIndex);
                }
                tokens = Tokenize(original);
                openTokenTypes = new List<TokenType>();
                current = new StringBuilder();
                int safeguard = 0;
                while (tokens.Count > 0 && safeguard < 10000) //!!!
                {
                    safeguard++;
                    if (!paused)
                    {
                        var deltaTime = DialogueTime.time - lastTime;

                        elapsed += deltaTime;
                        var goal = elapsed * charactersPerSecond;
                        var isCodeNext = false;
                        while (((charactersTyped < goal) || isCodeNext) && (tokens.Count > 0))
                        {
                            var token = GetNextToken(tokens);
                            switch (token.tokenType)
                            {
                                case TokenType.Character:
                                    if (rightToLeft)
                                    {
                                        current.Insert(0, token.character);
                                    }
                                    else
                                    {
                                        current.Append(token.character);
                                    }
                                    if (!IsSilentCharacter(token.character)) PlayCharacterAudio();
                                    onCharacter.Invoke();
                                    charactersTyped++;
                                    break;
                                case TokenType.BoldOpen:
                                case TokenType.ItalicOpen:
                                case TokenType.ColorOpen:
                                case TokenType.SizeOpen:
                                    OpenRichText(current, token, openTokenTypes);
                                    break;
                                case TokenType.BoldClose:
                                case TokenType.ItalicClose:
                                case TokenType.ColorClose:
                                case TokenType.SizeClose:
                                    CloseRichText(current, token, openTokenTypes);
                                    break;
                                case TokenType.Quad:
                                    current.Append(token.code);
                                    break;
                                case TokenType.Pause:
                                    control.text = GetCurrentText(current, openTokenTypes, tokens);
                                    paused = true;
                                    var continueTime = DialogueTime.time + token.duration;
                                    int pauseSafeguard = 0;
                                    while (DialogueTime.time < continueTime && pauseSafeguard < 999)
                                    {
                                        pauseSafeguard++;
                                        yield return null;
                                    }
                                    paused = false;
                                    break;
                                case TokenType.InstantOpen:
                                    AddInstantText(current, openTokenTypes, tokens);
                                    break;
                            }
                            isCodeNext = (tokens.Count > 0) && (tokens[0].tokenType != TokenType.Character);
                        }
                    }
                    // Set the text:
                    control.text = frontSkippedText + GetCurrentText(current, openTokenTypes, tokens);

                    // Handle auto-scrolling:
                    HandleAutoScroll();

                    //---Uncomment the line below to debug: 
                    //Debug.Log(control.text.Replace("<", "[").Replace(">", "]") + " " + name, this);

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

        private bool IsSilentCharacter(char c)
        {
            if (string.IsNullOrEmpty(silentCharacters)) return false;
            return silentCharacters.Contains(c.ToString());
        }

        private Token GetNextToken(List<Token> tokens)
        {
            if (tokens.Count == 0) return null;
            int lastIndex = rightToLeft ? tokens.Count - 1 : 0;
            var token = tokens[lastIndex];
            tokens.RemoveAt(lastIndex);
            return token;
        }

        private void OpenRichText(StringBuilder current, Token token, List<TokenType> openTokens)
        {
            switch (token.tokenType)
            {
                case TokenType.BoldOpen:
                    current.Append(RichTextBoldOpen);
                    break;
                case TokenType.ItalicOpen:
                    current.Append(RichTextItalicOpen);
                    break;
                case TokenType.ColorOpen:
                case TokenType.SizeOpen:
                    current.Append(token.code);
                    break;
            }
            openTokens.Insert(0, token.tokenType);
        }

        private void CloseRichText(StringBuilder current, Token token, List<TokenType> openTokens)
        {
            var openTokenType = TokenType.BoldOpen;
            switch (token.tokenType)
            {
                case TokenType.BoldClose:
                    current.Append(RichTextBoldClose);
                    openTokenType = TokenType.BoldOpen;
                    break;
                case TokenType.ItalicClose:
                    current.Append(RichTextItalicClose);
                    openTokenType = TokenType.ItalicOpen;
                    break;
                case TokenType.ColorClose:
                    current.Append(RichTextColorClose);
                    openTokenType = TokenType.ColorOpen;
                    break;
                case TokenType.SizeClose:
                    current.Append(RichTextSizeClose);
                    openTokenType = TokenType.SizeOpen;
                    break;
            }
            var first = -1;
            for (int i = 0; i < openTokens.Count; i++)
            {
                if (openTokens[i] == openTokenType)
                {
                    first = i;
                    break;
                }
            }
            if (first != -1) openTokens.RemoveAt(first);
        }

        private void AddInstantText(StringBuilder current, List<TokenType> openTokenTypes, List<Token> tokens)
        {
            int safeguard = 0;
            while ((tokens.Count > 0) && (safeguard < MaxSafeguard))
            {
                safeguard++;
                var token = GetNextToken(tokens);
                switch (token.tokenType)
                {
                    case TokenType.Character:
                        current.Append(token.character);
                        break;
                    case TokenType.BoldOpen:
                    case TokenType.ItalicOpen:
                    case TokenType.ColorOpen:
                    case TokenType.SizeOpen:
                        OpenRichText(current, token, openTokenTypes);
                        break;
                    case TokenType.BoldClose:
                    case TokenType.ItalicClose:
                    case TokenType.ColorClose:
                    case TokenType.SizeClose:
                        CloseRichText(current, token, openTokenTypes);
                        break;
                    case TokenType.InstantClose:
                        return;
                }
            }
        }

        private string GetCurrentText(StringBuilder current, List<TokenType> openTokenTypes, List<Token> tokens, bool withoutTransparentText = false)
        {
            if (current == null) return string.Empty;
            if (openTokenTypes == null || tokens == null) return current.ToString();

            // Start with the current text:
            var sb = new StringBuilder(current.ToString());

            // Close all open tags:
            for (int i = 0; i < openTokenTypes.Count; i++)
            {
                switch (openTokenTypes[i])
                {
                    case TokenType.BoldOpen:
                        sb.Append(RichTextBoldClose);
                        break;
                    case TokenType.ItalicOpen:
                        sb.Append(RichTextItalicClose);
                        break;
                    case TokenType.ColorOpen:
                        sb.Append(RichTextColorClose);
                        break;
                    case TokenType.SizeOpen:
                        sb.Append(RichTextSizeClose);
                        break;
                }
            }

            if (withoutTransparentText) return sb.ToString();

            // Add the rest as invisible text so size/alignment is correct:
            // Except don't add if auto-scrolling without a sizer text.
            var transparentText = new StringBuilder();
            var useTransparentText = !(autoScrollSettings.autoScrollEnabled && autoScrollSettings.sizerText == null);
            if (useTransparentText)
            {
                transparentText.Append("<color=#00000000>");
                for (int i = openTokenTypes.Count - 1; i >= 0; i--) // Reopen early-closed codes.
                {
                    switch (openTokenTypes[i])
                    {
                        case TokenType.BoldOpen:
                            transparentText.Append(RichTextBoldOpen);
                            break;
                        case TokenType.ItalicOpen:
                            transparentText.Append(RichTextItalicOpen);
                            break;
                    }
                }
                for (int i = 0; i < tokens.Count; i++)
                {
                    var token = tokens[i];
                    switch (token.tokenType)
                    {
                        case TokenType.BoldOpen:
                            transparentText.Append(RichTextBoldOpen);
                            break;
                        case TokenType.BoldClose:
                            transparentText.Append(RichTextBoldClose);
                            break;
                        case TokenType.ItalicOpen:
                            transparentText.Append(RichTextItalicOpen);
                            break;
                        case TokenType.ItalicClose:
                            transparentText.Append(RichTextItalicClose);
                            break;
                        case TokenType.ColorOpen:
                        case TokenType.ColorClose:
                        case TokenType.SizeOpen:
                        case TokenType.SizeClose:
                            break;
                        case TokenType.Character:
                            transparentText.Append(token.character);
                            break;
                    }
                }
                transparentText.Append("</color>");
                if (rightToLeft)
                    sb.Insert(0, transparentText);
                else
                    sb.Append(transparentText);
            }

            return sb.ToString();
        }

        private List<Token> Tokenize(string text)
        {
            var tokens = new List<Token>();
            var remainder = text;
            int safeguard = 0;
            while (!string.IsNullOrEmpty(remainder) && (safeguard < MaxSafeguard))
            {
                safeguard++;
                Token token = null;
                if (remainder[0].Equals('<')) //---Was: if (remainder.StartsWith(@"<"))
                {
                    token = TryTokenize(RichTextBoldOpen, TokenType.BoldOpen, 0, ref remainder);
                    if (token == null) token = TryTokenize(RichTextBoldClose, TokenType.BoldClose, 0, ref remainder);
                    if (token == null) token = TryTokenize(RichTextItalicOpen, TokenType.ItalicOpen, 0, ref remainder);
                    if (token == null) token = TryTokenize(RichTextItalicClose, TokenType.ItalicClose, 0, ref remainder);
                    if (token == null) token = TryTokenize(RichTextColorClose, TokenType.ColorClose, 0, ref remainder);
                    if (token == null) token = TryTokenize(RichTextSizeClose, TokenType.SizeClose, 0, ref remainder);
                    if (token == null) token = TryTokenizeColorOpen(ref remainder);
                    if (token == null) token = TryTokenizeSizeOpen(ref remainder);
                    if (token == null) token = TryTokenizeQuad(ref remainder);
                }
                else if (remainder[0].Equals('\\')) //---Was: else if (remainder.StartsWith(@"\"))
                {
                    // Check for RPGMaker-style codes:
                    token = TryTokenize(UITools.RPGMakerCodeFullPause, TokenType.Pause, fullPauseDuration, ref remainder);
                    if (token == null) token = TryTokenize(UITools.RPGMakerCodeQuarterPause, TokenType.Pause, quarterPauseDuration, ref remainder);
                    if (token == null) token = TryTokenize(UITools.RPGMakerCodeInstantOpen, TokenType.InstantOpen, 0, ref remainder);
                    if (token == null) token = TryTokenize(UITools.RPGMakerCodeInstantClose, TokenType.InstantClose, 0, ref remainder);
                    if (token == null) token = TryTokenize(UITools.RPGMakerCodeSkipToEnd, TokenType.InstantOpen, 0, ref remainder);
                }
                if (token == null)
                {
                    // Get regular character:
                    token = new Token(TokenType.Character, remainder[0], string.Empty, 0);
                    remainder = remainder.Remove(0, 1);
                }
                tokens.Add(token);
            }

            //--- Uncomment to debug tokenization:
            //string s = string.Empty;
            //for (int i = 0; i < tokens.Count; i++)
            //{
            //    s += tokens[i].tokenType + ": [" + tokens[i].character + "] : [" + tokens[i].code + "]\n";
            //}
            //Debug.Log(s);

            return tokens;
        }

        private Token TryTokenize(string code, TokenType tokenType, float duration, ref string remainder)
        {
            if (remainder.StartsWith(code, System.StringComparison.OrdinalIgnoreCase))
            {
                remainder = remainder.Remove(0, code.Length);
                return new Token(tokenType, ' ', string.Empty, duration);
            }
            else
            {
                return null;
            }
        }

        private Token TryTokenizeColorOpen(ref string remainder)
        {
            if (remainder.StartsWith(RichTextColorOpenPrefix))
            {
                var colorCode = remainder.Substring(0, remainder.IndexOf('>') + 1);
                remainder = remainder.Remove(0, colorCode.Length);
                return new Token(TokenType.ColorOpen, ' ', colorCode, 0);
            }
            else
            {
                return null;
            }
        }

        private Token TryTokenizeSizeOpen(ref string remainder)
        {
            if (remainder.StartsWith(RichTextSizeOpenPrefix))
            {
                var sizeCode = remainder.Substring(0, remainder.IndexOf('>') + 1);
                remainder = remainder.Remove(0, sizeCode.Length);
                return new Token(TokenType.SizeOpen, ' ', sizeCode, 0);
            }
            else
            {
                return null;
            }
        }

        private Token TryTokenizeQuad(ref string remainder)
        {
            if (remainder.StartsWith(QuadPrefix))
            {
                var quadCode = remainder.Substring(0, remainder.IndexOf('>') + 1);
                remainder = remainder.Remove(0, quadCode.Length);
                return new Token(TokenType.Quad, ' ', quadCode, 0);
            }
            else
            {
                return null;
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
            if (control != null) control.text = UITools.StripRPGMakerCodes(frontSkippedText + original);
            original = null;
            if (autoScrollSettings.autoScrollEnabled)
            {
                if (current != null && autoScrollSettings.sizerText != null)
                {
                    current = new StringBuilder(control.text);
                    if (enabled && gameObject.activeInHierarchy)
                    {
                        StartCoroutine(HandleAutoScrollAfterOneFrame()); // Need to give Unity UI one frame to update.
                    }
                }
                HandleAutoScroll();
            }
        }

        private void InitAutoScroll()
        {
            // Ensure sizer text alpha is 0:
            if (autoScrollSettings.autoScrollEnabled && autoScrollSettings.sizerText != null)
            {
                autoScrollSettings.sizerText.color = new Color(0, 0, 0, 0);
            }
        }

        private void HandleAutoScroll()
        {
            if (!autoScrollSettings.autoScrollEnabled) return;
            if (autoScrollSettings.sizerText != null)
            {
                autoScrollSettings.sizerText.text = GetCurrentText(current, openTokenTypes, tokens, true);
            }
            if (autoScrollSettings.scrollRect != null)
            {
                autoScrollSettings.scrollRect.normalizedPosition = Vector2.zero;
            }
            if (autoScrollSettings.scrollbarEnabler != null)
            {
                autoScrollSettings.scrollbarEnabler.CheckScrollbar();
            }
        }

        private IEnumerator HandleAutoScrollAfterOneFrame()
        {
            yield return null;
            HandleAutoScroll();
        }

    }

}
