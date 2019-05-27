// Copyright (c) 2018 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.Integration.NGUI
{

    using UnityEngine;
    using System;
    using System.Collections.Generic;

    [AddComponentMenu("RQ/UI/Rewired NGUI Bridge")]
    public class RewiredNGUIBridge : MonoBehaviour
    {

        [Serializable]
        public class KeyMapping
        {
            public KeyCode NGUIKey;
            public string rewiredAction;
        }

        [Serializable]
        public class AxisMapping
        {
            public string NGUIAxis;
            public string rewiredAction;
        }

        [Tooltip("The Rewired Player Id which will be used to control the UI.")]
        [SerializeField]
        private int _playerId;

        [SerializeField]
        private KeyMapping _submitAction = new KeyMapping() { NGUIKey = KeyCode.Return, rewiredAction = "UISubmit" };
        [SerializeField]
        private KeyMapping _cancelAction = new KeyMapping() { NGUIKey = KeyCode.Escape, rewiredAction = "UICancel" };
        [SerializeField]
        private AxisMapping _horizontalAction = new AxisMapping() { NGUIAxis = "Horizontal", rewiredAction = "UIHorizontal" };
        [SerializeField]
        private AxisMapping _verticalAction = new AxisMapping() { NGUIAxis = "Vertical", rewiredAction = "UIVertical" };
        [SerializeField]
        private AxisMapping _scrollAction = new AxisMapping() { NGUIAxis = "Mouse ScrollWheel", rewiredAction = "UIScroll" };

        public KeyMapping submitAction { get { return _submitAction; } }
        public KeyMapping cancelAction { get { return _cancelAction; } }
        public AxisMapping horizontalAction { get { return _horizontalAction; } }
        public AxisMapping verticalAction { get { return _verticalAction; } }
        public AxisMapping scrollAction { get { return _scrollAction; } }

        private Dictionary<int, int> _keyMappings;
        private Dictionary<string, int> _axisMappings;

        private Player player { get { return ReInput.isReady ? ReInput.players.GetPlayer(_playerId) : null; } }

        void Awake()
        {
            _keyMappings = new Dictionary<int, int>();
            _axisMappings = new Dictionary<string, int>();

            if (!ReInput.isReady)
            {
                Debug.LogError("Rewired has not been initialized. You must have an enabled Rewired Input Manager in the scene to use the NGUI Bridge");
                return;
            }

            InputAction action;

            action = GetAction(_submitAction.rewiredAction);
            if (action != null) _keyMappings.Add((int)_submitAction.NGUIKey, action.id);

            action = GetAction(_cancelAction.rewiredAction);
            if (action != null) _keyMappings.Add((int)_cancelAction.NGUIKey, action.id);

            action = GetAction(_horizontalAction.rewiredAction);
            if (action != null) _axisMappings.Add(_horizontalAction.NGUIAxis, action.id);

            action = GetAction(_verticalAction.rewiredAction);
            if (action != null) _axisMappings.Add(_verticalAction.NGUIAxis, action.id);

            action = GetAction(_scrollAction.rewiredAction);
            if (action != null) _axisMappings.Add(_scrollAction.NGUIAxis, action.id);
        }

        void Start()
        {
            UICamera.GetKey = GetKey;
            UICamera.GetKeyDown = GetKeyDown;
            UICamera.GetKeyUp = GetKeyUp;
            UICamera.GetAxis = GetAxis;
        }

        bool GetKeyDown(KeyCode key)
        {
            if (!ReInput.isReady) return false;
            int actionId;
            if (!_keyMappings.TryGetValue((int)key, out actionId)) return UnityEngine.Input.GetKeyDown(key);
            bool keyDown = player.GetButtonDown(actionId);
            if (keyDown)
            {
                int i = 1;
            }
            return keyDown;
        }

        bool GetKey(KeyCode key)
        {
            if (!ReInput.isReady) return false;
            int actionId;
            if (!_keyMappings.TryGetValue((int)key, out actionId)) return UnityEngine.Input.GetKey(key);
            return player.GetButton(actionId);
        }

        bool GetKeyUp(KeyCode key)
        {
            if (!ReInput.isReady) return false;
            int actionId;
            if (!_keyMappings.TryGetValue((int)key, out actionId)) return UnityEngine.Input.GetKeyUp(key);
            return player.GetButtonUp(actionId);
        }

        float GetAxis(string name)
        {
            if (!ReInput.isReady) return 0f;
            int actionId;
            if (!_axisMappings.TryGetValue(name, out actionId)) return 0f;
            return player.GetAxis(actionId);
        }

        InputAction GetAction(string name)
        {
            InputAction action = ReInput.mapping.GetAction(name);
            if (action == null) Debug.LogWarning("There is no Rewired Action named \"" + name + "\". You must create this Action in the Rewired Input Manager.");
            return action;
        }
    }
}